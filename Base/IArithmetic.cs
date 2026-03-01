
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TargetIntervals
{
    public interface IArithmetic<T>
        where T : IArithmetic<T>, IComparable<T>, IEquatable<T>, IFormattable
    {
        static abstract T Zero { get; }
        static abstract T Unit { get; }
        static abstract T InfinityPositive { get; }
        static abstract T InfinityNegative { get; }
        static abstract T operator +(T left, T right);
        static abstract T operator -(T left, T right);
        static abstract T operator *(T left, T right);
        static abstract T operator /(T left, T right);
    }
}