
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TargetIntervals.Base
{
    public interface IMeasurable<T>
        where T : IMeasurable<T>, IComparable<T>, IEquatable<T>, IFormattable
    {
        static abstract double Distance(T left, T right);
        static virtual bool operator <(T left, T right) => left.CompareTo(right) < 0;
        static virtual bool operator >(T left, T right) => left.CompareTo(right) > 0;
        static virtual bool operator <=(T left, T right) => left.CompareTo(right) <= 0;
        static virtual bool operator >=(T left, T right) => left.CompareTo(right) >= 0;
        static virtual bool operator ==(T? left, T? right) => Equals(left, right);
        static virtual bool operator !=(T? left, T? right) => !Equals(left, right);
    }
}