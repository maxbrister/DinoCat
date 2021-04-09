using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI
{
    public struct Rect : IEquatable<Rect>
    {
        public Rect(float left, float top, float right, float bottom) :
            this(new Point(left, top), new Size(right - left, bottom - top))
        { }

        public Rect(Size size) : this(new Point(), size)
        { }

        public Rect(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public float Left => Location.X;
        public float Top => Location.Y;
        public float Right => Location.X + Width;
        public float Bottom => Location.Y + Height;
        public Point Location { get; set; }
        public Size Size { get; set; }

        public float Width => Size.Width;
        public float Height => Size.Height;

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
