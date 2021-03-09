using System;

namespace DinoCat.Base
{
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

        public bool Equals(Size other) =>
            Width == other.Width && Height == other.Height;

        public override bool Equals(object? obj) =>
            obj is Size size && Equals(size);

        public override int GetHashCode() =>
            Width.GetHashCode() ^ Height.GetHashCode();
    }
}
