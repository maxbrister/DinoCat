using System;
using System.Diagnostics;

namespace DinoCat
{
    [DebuggerDisplay("{Width} x {Height}")]
    public struct Size : IEquatable<Size>
    {
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; set; }
        public double Height { get; set; }

        public static bool operator ==(Size left, Size right) =>
            left.Equals(right);

        public static bool operator !=(Size left, Size right) =>
            !(left == right);

        public double this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return Width;
                    case 1:
                        return Height;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        Width = value;
                        break;
                    case 1:
                        Height = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public bool Equals(Size other) =>
            Width == other.Width && Height == other.Height;

        public override bool Equals(object? obj) =>
            obj is Size size && Equals(size);

        public override int GetHashCode() =>
            Width.GetHashCode() ^ Height.GetHashCode();
    }
}
