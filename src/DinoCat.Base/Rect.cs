using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base
{
    public struct Rect : IEquatable<Rect>
    {
        public Rect(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rect(Size size)
        {
            Left = 0;
            Top = 0;
            Right = size.Width;
            Bottom = size.Height;
        }

        public Rect(Point location, Size size)
        {
            Left = location.X;
            Top = location.Y;
            Right = location.X + size.Width;
            Bottom = location.Y + size.Height;
        }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public Point Location
        {
            get => new Point(Left, Top);
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }
        public Size Size
        {
            get => new Size(Right - Left, Bottom - Top);
            set
            {
                Right = Left + value.Width;
                Bottom = Top + value.Height;
            }
        }

        public double Width => Right - Left;
        public double Height => Bottom - Top;

        public static bool operator ==(Rect left, Rect right) =>
            left.Equals(right);

        public static bool operator !=(Rect left, Rect right) =>
            !(left == right);

        public bool Contains(Point p) =>
            p.X >= Left && p.Y >= Top && p.X <= Right && p.Y <= Bottom;

        public bool Equals(Rect other) =>
            Location == other.Location && Size == other.Size;

        public override bool Equals(object? obj) =>
            obj is Rect r && Equals(r);

        public override int GetHashCode() =>
            Location.GetHashCode() ^ Size.GetHashCode();
    }
}
