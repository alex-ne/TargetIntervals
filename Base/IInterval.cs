using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetIntervals.Base
{
    public interface IInterval<T, TSelf>
        where T : IMeasurable<T>, IComparable<T>, IEquatable<T>, IFormattable
        where TSelf : IInterval<T, TSelf>
    {
        static abstract TSelf Create(T end1, T end2);
        T Head { get; set; }
        T Tail { get; set; }
        double Size => T.Distance(Head, Tail);
        bool IsEmpty => Head == null || Tail == null;
        void Initialize(T end1, T end2)
        {
            if (end1 is null || end2 is null)
            {
                Head = default!;
                Tail = default!;
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
        void Clear()
        {
            Initialize(default!, default!);
        }
        bool Contains(T point)
        {
            return !IsEmpty && !(point is null)
                ? Head.CompareTo(point) <= 0 && point.CompareTo(Tail) < 0
                : false;
        }
        bool Contains(IInterval<T, TSelf> sub)
        {
            return !IsEmpty && !(sub is null) && !sub.IsEmpty
                ? Contains(sub.Head) && sub.Tail.CompareTo(Tail) < 0
                : false;
        }
        void Merge(IInterval<T, TSelf> other)
        {
            if
                (
                    !IsEmpty && !other.IsEmpty
                    &&
                    (Contains(other.Head) || Contains(other.Tail) || other.Contains(Head) || other.Contains(Tail))
                )
            {
                T h = Head.CompareTo(other.Head) > 0 ? other.Head : Head;
                T t = Tail.CompareTo(other.Tail) < 0 ? other.Tail : Tail;
                Initialize(h, t);
            }
            else
            {
                Initialize(default, default);
            }
        }

        (TSelf? Left, TSelf? Right) Split(T point)
        {
            if (IsEmpty)
                return (default, default); // default вернет null для классов или пустое значение для структур

            if (Contains(point))
            {
                return (TSelf.Create(Head, point), TSelf.Create(point, Tail));
            }

            if (point.CompareTo(Head) < 0)
                return (default, (TSelf)this);

            return ((TSelf)this, default);
        }

        void Cross(IInterval<T, TSelf> other)
        {
            if
                (
                    !IsEmpty && !other.IsEmpty
                    &&
                    (Contains(other.Head) || Contains(other.Tail) || other.Contains(Head) || other.Contains(Tail))
                )
            {
                T h = Head.CompareTo(other.Head) > 0 ? Head : other.Head;
                T t = Tail.CompareTo(other.Tail) < 0 ? Tail : other.Tail;
                Initialize(h, t);
            }
            else
            {
                Initialize(default, default);
            }
        }
        void Substr(IInterval<T, TSelf> other)
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
        TSelf Clone()
        {
            return TSelf.Create(Head, Tail);
        }
        static TSelf operator |(IInterval<T, TSelf> left, IInterval<T, TSelf> right)
        {
            TSelf result = left.Clone();
            result.Merge(right);
            return result;
        }
        static TSelf operator ^(IInterval<T, TSelf> left, IInterval<T, TSelf> right)
        {
            TSelf result = left.Clone();
            result.Substr(right);
            return result;
        }

        static TSelf operator &(IInterval<T, TSelf> left, IInterval<T, TSelf> right)
        {
            TSelf result = left.Clone();
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
    }

}



