using DinoCat.Drawing;
using DinoCat.Tree;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Elements
{
    public class Row : Container<Expand>
    {
        public Row(VerticalAlignment alignment, FlowDirection? flowDirection, params Expand[] children) : base(children)
        {
            Alignment = alignment;
            FlowDirection = flowDirection;
        }
        public Row(VerticalAlignment alignment, params Expand[] children) : this(alignment, null, children) { }

        public Row(FlowDirection? flowDirection, params Expand[] children) : this(VerticalAlignment.Top, flowDirection, children) { }

        public Row(params Expand[] children) : this(VerticalAlignment.Top, children)
        { }

        public VerticalAlignment Alignment { get; }
        public FlowDirection? FlowDirection { get; }

        public override (Size, float?) Arrange(Context context, Size availableSize, List<Node> children)
        {
            var direction = FlowDirection ?? context.Get<FlowDirection>();
            var zipped = children.Zip(Children, (a, b) => (a, b));
            if (direction == Elements.FlowDirection.RightToLeft)
                zipped = zipped.Reverse();

            var scale = context.Get<DpiScale>();
            var layout = LinearLayout.Horizontal(scale, Alignment, direction);
            return layout.Arrange(availableSize, zipped);
        }

        public override bool IsLayoutInvalid(Container<Expand> oldContainer)
        {
            var oldRow = (Row)oldContainer;
            if (Alignment != oldRow.Alignment)
                return true;
            return Children.Zip(oldRow.Children, (a, b) => (a, b)).Any(pair => pair.a.Factor != pair.b.Factor);
        }

        public override Element ToElement(Expand child) => child.Child;
    }
}
