using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetIntervals.Base;

namespace TargetIntervals
{
    public class DateCovering : IntervalCoveringBase<MetrDateTime, DateInterval>
    {
        public DateCovering() 
        {
            _tops = new List<MetrDateTime>();
            _intervals = new SortedList<DateInterval, double?>();
            _bounds = new DateInterval();
        }
        public override void Initialize(object args)
        { 
        }
        public DateInterval Bounds => _bounds;
        public SortedList<DateInterval, double?> Intervals
        {
            get { return _intervals; }
            set { UpdateIntervals(value); }
        }
        public List<MetrDateTime> Tops
        {
            get { return _tops; }
            set { UpdateTops(value); }
        }
        public void UpdateTops(List<MetrDateTime> tops)
        {
            _tops.Clear();
            _intervals.Clear();
            _bounds.Clear();

            MetrDateTime pp = default;
            foreach (MetrDateTime p in tops.OrderBy(p => p))
            {
                if (pp != p && p is not null)
                {
                    _tops.Add(p);
                    pp = p;
                }
            }
            if (_tops.Count < 2)
            {
                _tops.Clear();
                return;
            }

            UpdateBounds(_tops);
            if (_bounds.IsEmpty) return;
            int n = _tops.Count - 2;
            for (int i = 0; i <= n; i++)
            {
                _intervals.Add(DateInterval.Create(_tops[i], _tops[i + 1]), null);
            }
        }
        public void UpdateIntervals(SortedList<DateInterval, double?> intervals)

        {
            _tops.Clear();
            _intervals.Clear();
            _bounds.Clear();
            if (intervals.Count > 0)
            {
                foreach (var p in intervals.OrderBy(p => p.Key.Head))
                {
                    if (p.Key.IsEmpty) continue;
                    _intervals.Add(p.Key, p.Value);
                    _tops.Add(p.Key.Head);
                }
                if (_intervals.Count > 0)
                {
                    _tops.Add(_intervals.Keys[_intervals.Count - 1].Tail);
                    UpdateBounds(_tops);
                }
            }
        }
        public Func<double?, double?, double?> OnConvolution { get; set; }
        public TCovering Cross<TCovering>(TCovering other)
            where TCovering : IntervalCoveringBase<MetrDateTime, DateInterval>, new()
        {
            if (Tops.Count < 2 || other.Tops.Count < 2) return null;

            TCovering result = new TCovering();
            // Используем ToList(), так как Union возвращает последовательность
            List<MetrDateTime> newTops = Tops.Union(other.Tops).ToList();
            result.UpdateTops(newTops);

            for (int i = 0; i < result.Intervals.Count; i++)
            {
                var currentKey = result.Intervals.Keys[i];

                // Ищем значения в исходных наборах
                var val1 = FindValue(Intervals, currentKey);
                var val2 = FindValue(other.Intervals, currentKey);

                // Присваиваем результат свертки (OnConvolution должен быть готов к null!)
                result.Intervals[currentKey] = OnConvolution?.Invoke(val1, val2);
            }
            return result;
        }
        // Вспомогательный метод поиска значения для интервала
        private double? FindValue(SortedList<DateInterval, double?> intervals, DateInterval target)
        {
            double? result = null;
            int k = (intervals.Count - 1) / 2;
            int dk = (k + 1) / 2;
            DateInterval iv;
            while (k >= 0 && k < intervals.Count && dk > 0)
            {
                iv = intervals.Keys[k];
                if (iv.Head > target.Head)
                {
                    k = k - dk;
                }
                else if (iv.Tail < target.Tail)
                {
                    k = k + dk;
                }
                else if (iv.Contains(target))
                {
                    result = intervals.Values[k];
                    break;
                }
                dk = dk / 2;
            }
            return result;
        }
        private void UpdateBounds(List<MetrDateTime> tops)
        {
            MetrDateTime h = null, t = null;
            if (tops.Count() >= 2)
            {
                h = tops[0];
                t = tops[tops.Count - 1];
            }
            if (_bounds is null)
                _bounds = DateInterval.Create(h, t);
            else
                _bounds.Initialize(h, t);
        }

        private List<MetrDateTime> _tops = new List<MetrDateTime>();
        private SortedList<DateInterval, double?> _intervals = new SortedList<DateInterval, double?>();
        private DateInterval _bounds;
    }
}

