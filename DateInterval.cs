using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetIntervals.Base;

namespace TargetIntervals
{
    public class DateInterval : IInterval<MetrDateTime, DateInterval>
    {
        public DateInterval() 
        {
            Initialize(null, null);
        }
        public DateInterval(DateTime Head, DateTime Tile)
        {
            Initialize(new MetrDateTime(Head), new MetrDateTime(Tile));
        }
        public MetrDateTime Head { get; set; }
        public MetrDateTime Tail { get; set; }
        public double Size => MetrDateTime.Distance(Head, Tail);
        public bool IsEmpty => Head is null || Tail is null;
        public static DateInterval Create(MetrDateTime? end1, MetrDateTime? end2)
        {
            var interval = new DateInterval();
            interval.Initialize(end1, end2); 
            return interval;
        }
        public void Initialize(MetrDateTime? end1, MetrDateTime? end2)
        {
            if (end1 is null || end2 is null)
            {
                Head = null!;
                Tail = null!;
                return;
            }
            if (end1.CompareTo(end2) <= 0)
            {
                Head = end1;
                Tail = end2;
            }
            else
            {
                Head = end2;
                Tail = end1;
            }
        }
        public void Clear()
        {
            Initialize(default!, default!);
        }
        public bool Contains(DateTime date)
        {
            return Contains(new MetrDateTime(date));
        }
        public bool Contains(MetrDateTime point)
        {
            return !IsEmpty && !(point is null)
                ? Head.CompareTo(point) <= 0 && point.CompareTo(Tail) < 0
                : false;
        }
        public bool Contains(IInterval<MetrDateTime, DateInterval> sub)
        {
            return !IsEmpty && !(sub is null) && !sub.IsEmpty
                ? Contains(sub.Head) && sub.Tail.CompareTo(Tail) <= 0
                : false;
        }
        public void Merge(IInterval<MetrDateTime, DateInterval> other)
        {
            if
                (
                    !IsEmpty && !other.IsEmpty
                    &&
                    (Contains(other.Head) || Contains(other.Tail) || other.Contains(Head) || other.Contains(Tail))
                )
            {
                MetrDateTime h = Head.CompareTo(other.Head) > 0 ? other.Head : Head;
                MetrDateTime t = Tail.CompareTo(other.Tail) < 0 ? other.Tail : Tail;
                Initialize(h, t);
            }
            else
            {
                Initialize(default, default);
            }
        }
        public int CompareTo(DateInterval? other)
        {
            if (other is null) return 1;
            if (IsEmpty && other.IsEmpty) return 0;
            if (IsEmpty) return -1;
            if (other.IsEmpty) return 1;

            int res = Head.CompareTo(other.Head);
            if (res == 0) res = Tail.CompareTo(other.Tail);
            return res;
        }
        public bool Equals(DateInterval? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            // Интервалы равны, если равны их границы
            return Equals(Head, other.Head) && Equals(Tail, other.Tail);
        }
        public override bool Equals(object? obj) => Equals(obj as DateInterval);
        public override int GetHashCode()
        {
            // Используем HashCode.Combine для надежного хеширования границ
            return HashCode.Combine(Head, Tail);
        }
        public (DateInterval? Left, DateInterval? Right) Split(MetrDateTime point)
        {
            if (IsEmpty)
                return (default, default); // default вернет null для классов или пустое значение для структур

            if (Contains(point))
            {
                return (DateInterval.Create(Head, point), DateInterval.Create(point, Tail));
            }

            if (point.CompareTo(Head) < 0)
                return (default, (DateInterval)this);

            return ((DateInterval)this, default);
        }

        public void Cross(IInterval<MetrDateTime, DateInterval> other)
        {
            if
                (
                    !IsEmpty && !other.IsEmpty
                    &&
                    (Contains(other.Head) || Contains(other.Tail) || other.Contains(Head) || other.Contains(Tail))
                )
            {
                MetrDateTime h = Head.CompareTo(other.Head) > 0 ? Head : other.Head;
                MetrDateTime t = Tail.CompareTo(other.Tail) < 0 ? Tail : other.Tail;
                Initialize(h, t);
            }
            else
            {
                Initialize(default, default);
            }
        }
        public void Substr(IInterval<MetrDateTime, DateInterval> other)
        {
            if (IsEmpty || other.IsEmpty) { return; }
            if (Contains(other.Head) && !Contains(other.Tail))
            {
                Tail = other.Head;
            }
            else
            if (!Contains(other.Head) && Contains(other.Tail))
            {
                Head = other.Tail;
            }
            else
            {
                Initialize(default, default);
            }
        }
        public DateInterval Clone()
        {
            return DateInterval.Create(Head, Tail);
        }
        public static DateInterval operator |(DateInterval left, DateInterval right)
        {
            DateInterval result = left.Clone();
            result.Merge(right);
            return result;
        }
        public static DateInterval operator ^(DateInterval left, DateInterval right)
        {
            DateInterval result = left.Clone();
            result.Substr(right);
            return result;
        }

        public static DateInterval operator &(DateInterval left, DateInterval right)
        {
            DateInterval result = left.Clone();
            result.Cross(right);
            return result;
        }
        string ToString(string? format = null, IFormatProvider? formatProvider = null)
        {
            if (IsEmpty) return "[empty]";

            // Используем метод ToString из IFormattable, который мы потребовали у T
            string h = Head.ToString(format, formatProvider);
            string t = Tail.ToString(format, formatProvider);

            return $"[{h}; {t})";
        }
        
        private MetrDateTime _head;
        private MetrDateTime _tail;
    }
}
