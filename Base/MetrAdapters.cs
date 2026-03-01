using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TargetIntervals.Base
{
    public enum TimeUnit { Days, Hours, Minutes, Seconds }
    public class MetrDateTime : IMeasurable<MetrDateTime>, IComparable<MetrDateTime>, IEquatable<MetrDateTime>, IFormattable
    {
        private readonly DateTime _dt;

        public MetrDateTime(DateTime dt) => _dt = dt;
        public DateTime Value => _dt;
        // Реализация статического абстрактного метода интерфейса
        public static double Distance(MetrDateTime left, MetrDateTime right)
        {
            if (left is null || right is null) return 0;
            TimeSpan span = left._dt - right._dt;
            return DefaultUnit switch
            {
                TimeUnit.Days => Math.Abs(span.TotalDays),
                TimeUnit.Hours => Math.Abs(span.TotalHours),
                TimeUnit.Minutes => Math.Abs(span.TotalMinutes),
                _ => Math.Abs(span.TotalSeconds)
            };
        }

        // Классу необходимы явные перегрузки операторов (интерфейс их не заменит для обычного использования)
        public static bool operator <(MetrDateTime? left, MetrDateTime? right) => left?.CompareTo(right) < 0;
        public static bool operator >(MetrDateTime? left, MetrDateTime? right) => left?.CompareTo(right) > 0;
        public static bool operator ==(MetrDateTime? left, MetrDateTime? right) => Equals(left, right);
        public static bool operator !=(MetrDateTime? left, MetrDateTime? right) => !Equals(left, right);
        public int CompareTo(MetrDateTime? other) => other is null ? 1 : _dt.CompareTo(other._dt);
        public bool Equals(MetrDateTime? other) => other is not null && _dt.Equals(other._dt);
        public override bool Equals(object? obj) => obj is MetrDateTime other && Equals(other);
        public override int GetHashCode() => _dt.GetHashCode();
        public string ToString(string? format, IFormatProvider? formatProvider) => _dt.ToString(format, formatProvider);
        public static TimeUnit DefaultUnit { get; set; } = TimeUnit.Days;
    }
    /*
    ************************************* СТРУКТУРЫ *********************************
    public struct MetrDouble : IMeasurable<MetrDouble>, IArithmetic<MetrDouble>, IComparable<MetrDouble>, IEquatable<MetrDouble>, IFormattable
    {
        private readonly double _val;
        private static MetrDouble _zero = new(0);
        private static MetrDouble _unit = new(1);
        private static MetrDouble _pinf = new(double.PositiveInfinity);
        private static MetrDouble _ninf = new(double.NegativeInfinity);


        public static MetrDouble Zero => _zero;
        public static MetrDouble Unit => _unit;
        public static MetrDouble InfinityPositive => _pinf;
        public static MetrDouble InfinityNegative => _ninf;
        public MetrDouble(double v) => _val = v;

        public static implicit operator MetrDouble(double v) => new(v);
        public static implicit operator double(MetrDouble ad) => ad._val;

        // IArithmetic
        public static double Distance(MetrDouble left, MetrDouble right) => Math.Abs(left._val - right._val);
        public static MetrDouble operator +(MetrDouble a, MetrDouble b) => a._val + b._val;
        public static MetrDouble operator -(MetrDouble a, MetrDouble b) => a._val - b._val;
        public static MetrDouble operator *(MetrDouble a, MetrDouble b) => a._val * b._val;
        public static MetrDouble operator /(MetrDouble a, MetrDouble b) => a._val / b._val;

        // Сравнения и равенство
        public int CompareTo(MetrDouble other) => _val.CompareTo(other._val);
        public bool Equals(MetrDouble other) => _val.Equals(other._val);

        // Форматирование
        public string ToString(string? f, IFormatProvider? p) => _val.ToString(f, p);
        public override string ToString() => _val.ToString();
        public override bool Equals(object? obj) => obj is MetrDouble d && Equals(d);
        public override int GetHashCode() => _val.GetHashCode();
    }

    public struct MetrInt : IMeasurable<MetrInt>, IArithmetic<MetrInt>, IComparable<MetrInt>, IEquatable<MetrInt>, IFormattable
    {
        private readonly int _val;
        private static readonly MetrInt _zero = new(0);
        private static readonly MetrInt _unit = new(1);
        private static readonly MetrInt _pinf = new(int.MaxValue);
        private static readonly MetrInt _ninf = new(int.MinValue);

        public MetrInt(int v) => _val = v;

        // Реализация статических свойств IArithmetic
        public static MetrInt Zero => _zero;
        public static MetrInt Unit => _unit;
        public static MetrInt InfinityPositive => _pinf;
        public static MetrInt InfinityNegative => _ninf;

        public static implicit operator MetrInt(int v) => new(v);
        public static implicit operator int(MetrInt mi) => mi._val;

        // IMeasurable
        public static double Distance(MetrInt left, MetrInt right) => Math.Abs((double)left._val - right._val);

        // IArithmetic
        public static MetrInt operator +(MetrInt a, MetrInt b) => a._val + b._val;
        public static MetrInt operator -(MetrInt a, MetrInt b) => a._val - b._val;
        public static MetrInt operator *(MetrInt a, MetrInt b) => a._val * b._val;
        public static MetrInt operator /(MetrInt a, MetrInt b) => a._val / b._val;

        // Сравнения, равенство и форматирование
        public int CompareTo(MetrInt other) => _val.CompareTo(other._val);
        public bool Equals(MetrInt other) => _val.Equals(other._val);
        public override bool Equals(object? obj) => obj is MetrInt other && Equals(other);
        public override int GetHashCode() => _val.GetHashCode();
        public string ToString(string? f, IFormatProvider? p) => _val.ToString(f, p);
        public override string ToString() => _val.ToString();

        public static bool operator ==(MetrInt left, MetrInt right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MetrInt left, MetrInt right)
        {
            return !(left == right);
        }
    }

    public enum TimeUnit { Days, Hours, Minutes, Seconds }

    public struct MetrDateTime : IMeasurable<MetrDateTime>, IComparable<MetrDateTime>, IEquatable<MetrDateTime>, IFormattable
    {
        private readonly DateTime _dt;
        public static TimeUnit DefaultUnit { get; set; } = TimeUnit.Days;

        public static DateTime Base { get; set; } = new DateTime(2000, 1, 1);

        public MetrDateTime(DateTime dt) => _dt = dt;

        public static double Distance(MetrDateTime left, MetrDateTime right)
        {
            TimeSpan span = left._dt - right._dt;
            return DefaultUnit switch
            {
                TimeUnit.Days => Math.Abs(span.TotalDays),
                TimeUnit.Hours => Math.Abs(span.TotalHours),
                TimeUnit.Minutes => Math.Abs(span.TotalMinutes),
                _ => Math.Abs(span.TotalSeconds)
            };
        }

        public int CompareTo(MetrDateTime other)
        {
            return _dt.CompareTo(other._dt);
        }

        // 2. Реализация IEquatable<ArithDateTime>
        public bool Equals(MetrDateTime other)
        {
            return _dt.Equals(other._dt);
        }
        public static MetrDateTime operator +(MetrDateTime a, double b) =>
            new(DefaultUnit switch
            {
                TimeUnit.Days => a._dt.AddDays(b),
                TimeUnit.Hours => a._dt.AddHours(b),
                TimeUnit.Minutes => a._dt.AddMinutes(b),
                _ => a._dt.AddSeconds(b)
            });
        public static MetrDateTime operator -(MetrDateTime a, double b) => a + -b;

        // 4. Реализация IFormattable
        public string ToString(string? format, IFormatProvider? formatProvider) => _dt.ToString(format, formatProvider);

        // Не забывайте стандартные переопределения для структур
        public override bool Equals(object? obj) => obj is MetrDateTime other && Equals(other);
        public override int GetHashCode() => _dt.GetHashCode();
        public override string ToString() => _dt.ToString();
    }
    */

}
