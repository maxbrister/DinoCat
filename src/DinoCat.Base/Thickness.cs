using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat
{
    public struct Thickness : IEquatable<Thickness>
    {
        public Thickness(double uniform) : this(uniform, uniform, uniform, uniform)
        {}

        public Thickness(double left, double top) : this(left, top, 0, 0)
        { }

        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public double Left { get; }
        public double Top { get; }
        public double Right { get; }
        public double Bottom { get; }

        public bool Equals(Thickness other) =>
            Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;

        public override bool Equals(object? obj) =>
            obj is Thickness t && Equals(t);

        public static bool operator ==(Thickness left, Thickness right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Thickness left, Thickness right)
        {
            return !(left == right);
        }

        public static Thickness operator +(Thickness a, Thickness b) =>
            new(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);

        public static Thickness operator -(Thickness a, Thickness b) =>
            new(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);

        public override int GetHashCode()
        {
#if NETCOREAPP
            return HashCode.Combine(Left, Top, Right, Bottom);
#else
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
#endif
        }
    }
}
