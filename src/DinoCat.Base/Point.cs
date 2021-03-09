using System;

namespace DinoCat.Base
{
    public struct Point : IEquatable<Point>
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public static bool operator ==(Point left, Point right) =>
            left.Equals(right);

        public static bool operator !=(Point left, Point right) =>
            !(left == right);

        public bool Equals(Point other) =>
            X == other.X && Y == other.Y;

        public override bool Equals(object? obj) =>
            obj is Point point && Equals(point);

        public override int GetHashCode() =>
            X.GetHashCode() ^ Y.GetHashCode();
    }
}
