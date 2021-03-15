namespace DinoCat.Elements
{
    public class Expand
    {
        public Expand(Element child, int flex = 1)
        {
            Child = child;
            Flex = flex;
        }

        private Expand(Expand e, double? x = null, double? y = null)
        {
            Child = e.Child;
            X = x ?? e.X;
            Y = y ?? e.Y;
            Flex = e.Flex;
        }

        private Expand(Aligned a)
        {
            Child = a.Child;
            X = a.X;
            Y = a.Y;
            Flex = 0;
        }

        public static implicit operator Expand(Aligned a) => new Expand(a);
        public static implicit operator Expand(Element e) => new Expand(e, flex: 0);

        public Element Child { get; }
        public double? X { get; }
        public double? Y { get; }
        public int Flex { get; }

        public Expand Align(double? x = null, double? y = null) => new Expand(this, x, y);
        public Expand Left() => new Expand(this, 0, null);
        public Expand Top() => new Expand(this, null, 0);
        public Expand Right() => new Expand(this, 1, null);
        public Expand Bottom() => new Expand(this, null, 1);
        public Expand TopLeft() => new Expand(this, 0, 0);
        public Expand TopRight() => new Expand(this, 1, 0);
        public Expand BottomLeft() => new Expand(this, 0, 1);
        public Expand BottomRight() => new Expand(this, 1, 1);
        public Expand Center() => new Expand(this, 0.5, 0.5);
    }

    public static class ExpandHelper
    {
        public static Expand Expand(this Element e, int flex = 1) => new Expand(e, flex);
    }
}
