using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TargetIntervals.Base
{
    public abstract class IntervalCoveringBase<T, TInterval>
        where T : IMeasurable<T>, IComparable<T>, IEquatable<T>, IFormattable
        where TInterval : class, IInterval<T, TInterval>
    {
        public abstract void Initialize(object args);
        protected List<T> _tops = new List<T>();
        protected SortedList<TInterval, double?> _intervals = new SortedList<TInterval, double?>();
        protected TInterval _bounds;
        public TInterval Bounds => _bounds;
        public SortedList<TInterval, double?> Intervals
        {
            get { return _intervals; }
            set { UpdateIntervals(value); }
        }
        public List<T> Tops
        {
            get { return _tops; }
            set { UpdateTops(value); }
        }
        public void UpdateTops(List<T> tops)
        {
            _tops.Clear();
            _intervals.Clear();
            _bounds.Clear();

            T pp = default;
            foreach (T p in tops.OrderBy(p => p))
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
                _intervals.Add(TInterval.Create(_tops[i], _tops[i + 1]), null);
            }
        }
        public void UpdateIntervals(SortedList<TInterval, double?> intervals)

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
            where TCovering : IntervalCoveringBase<T, TInterval>, new()
        {
            if (Tops.Count < 2 || other.Tops.Count < 2) return null;

            TCovering result = new TCovering();
            // Используем ToList(), так как Union возвращает последовательность
            List<T> newTops = Tops.Union(other.Tops).ToList();
            result.UpdateTops(newTops);

            for (int i = 0; i < result._intervals.Count; i++)
            {
                var currentKey = result._intervals.Keys[i];

                // Ищем значения в исходных наборах
                var val1 = FindValue(_intervals, currentKey);
                var val2 = FindValue(other._intervals, currentKey);

                // Присваиваем результат свертки (OnConvolution должен быть готов к null!)
                result._intervals[currentKey] = OnConvolution?.Invoke(val1, val2);
            }
            return result;
        }
        // Вспомогательный метод поиска значения для интервала
        private double? FindValue(SortedList<TInterval, double?> intervals, TInterval target)
        {
            double? result = null;
            int k = (intervals.Count - 1) / 2;
            int dk = (k + 1) / 2;
            TInterval iv;
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
        private void UpdateBounds(List<T> tops)
        {
            T h = default, t = default;
            if (tops.Count() >= 2)
            {
                h = tops[0];
                t = tops[tops.Count - 1];
            }
            if (_bounds is null)
                _bounds = TInterval.Create(h, t);
            else
                _bounds.Initialize(h, t);
        }
    }
}
