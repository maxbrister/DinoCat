using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat
{
    public struct Rect : IEquatable<Rect>
    {
        public Rect(double left, double top, double right, double bottom) :
            this(new Point(left, top), new Size(right - left, bottom - top))
        { }

        public Rect(Size size) : this(new Point(), size)
        { }

        public Rect(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public double Left => Location.X;
        public double Top => Location.Y;
        public double Right => Location.X + Width;
        public double Bottom => Location.Y + Height;
        public Point Location { get; set; }
        public Size Size { get; set; }

        public double Width => Size.Width;
        public double Height => Size.Height;

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
