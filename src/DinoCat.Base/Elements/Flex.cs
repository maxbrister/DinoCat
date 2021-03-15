using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Elements
{
    public class Flex : Container
    {
        public Flex(Orientation orientation, params Expand[] children) : base(children.Select(c => c.Child).ToArray())
        {
            Expand = children;
            Orientation = orientation;
        }

        public IReadOnlyList<Expand> Expand { get; }
        public Orientation Orientation { get; }

        public override Size Arrange(Size availableSize, List<Node> children)
        {
            double remaining = availableSize[MainAxis];
            double availableCross = availableSize[CrossAxis];
            int totalFlex = 0;
            int expandCount = 0;

            // Layout non-flex children allowing them to take up all available space.
            for (int i = 0; i < Children.Count; ++i)
            {
                var expand = Expand[i];
                if (expand.Flex != 0)
                {
                    totalFlex += expand.Flex;
                    ++expandCount;
                }
                else
                {
                    var childAvailable = MakeSize(remaining, availableCross);
                    var childActual = children[i].Arrange(childAvailable);
                    remaining = Math.Max(remaining - childActual[MainAxis], 0);
                }
            }

            if (expandCount > 0)
            {
                // Layout flex children dividing up the remaining space
                double spacePerUnit = totalFlex != 0 ? remaining / totalFlex : 0;
                for (int i = 0; i < Children.Count; ++i)
                {
                    var expand = Expand[i];
                    if (expand.Flex != 0)
                    {
                        double space = spacePerUnit * expand.Flex;
                        var childSize = MakeSize(space, availableCross);
                        children[i].Arrange(childSize);
                    }
                }
            }

            // Place children along the main axis
            double pos = 0;
            double crossExtents = 0;
            for (int i = 0; i < children.Count; ++i)
            {
                var size = children[i].Size;
                var offset = MakePoint(pos, 0);
                children[i].Offset = offset;
                pos += size[MainAxis];
                crossExtents = Math.Max(crossExtents, size[CrossAxis]);
            }

            for (int i = 0; i < Children.Count; ++i)
            {
                var child = children[i];
                var size = child.Size;
                size[CrossAxis] = crossExtents;

                var expand = Expand[i];
                Aligned.AlignNode(child, size, expand.X, expand.Y);
            }

            var flexSize = MakeSize(pos, crossExtents);
            return flexSize;
        }

        public override bool IsLayoutInvalid(Container oldContainer) =>
            ((Flex)oldContainer).Orientation != Orientation;

        private Point MakePoint(double mainAxisValue, double crossAxisValue)
        {
            var point = new Point();
            point[MainAxis] = mainAxisValue;
            point[CrossAxis] = CrossAxis;
            return point;
        }

        private Size MakeSize(double mainAxisValue, double crossAxisValue)
        {
            var size = new Size();
            size[MainAxis] = mainAxisValue;
            size[CrossAxis] = crossAxisValue;
            return size;
        }

        private int MainAxis => Orientation == Orientation.Horizontal ? 0 : 1;
        private int CrossAxis => (MainAxis + 1) % 2;
    }
}
