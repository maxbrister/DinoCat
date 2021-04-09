using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Tree;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.StandardUI.Elements
{
    public class Column : Container<Expand>
    {
        public Column(HorizontalAlignment alignment, params Expand[] children) : base(children) =>
            Alignment = alignment;
        public Column(params Expand[] children) : this(HorizontalAlignment.Left, children) { }

        public HorizontalAlignment Alignment { get; }

        public override (Size, float?) Arrange(Context context, Size availableSize, List<Node> children)
        {
            var scale = context.Get<DpiScale>();
            var flow = context.Get<FlowDirection>();
            var layout = LinearLayout.Vertical(scale, Alignment, flow);
            return layout.Arrange(availableSize, children.Zip(Children, (a, b) => (a, b)));
        }

        public override bool IsLayoutInvalid(Container<Expand> oldContainer)
        {
            var oldColumn = (Column)oldContainer;
            if (oldColumn.Alignment != Alignment)
                return true;
            return Children.Zip(oldColumn.Children, (a, b) => (a, b)).Any(pair => pair.a.Factor != pair.b.Factor);
        }

        public override Element ToElement(Expand child) => child.Child;
    }
}
