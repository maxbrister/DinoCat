using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Tree
{
    public class DrawingNode : NodeBase<DrawingElementBase>
    {
        public DrawingNode(Node? parent, Context context, DrawingElementBase element) : base(parent, context, element) { }

        public override IEnumerable<Node> Children =>
            Enumerable.Empty<Node>();

        protected override (Size, float?) ArrangeOverride(Size availableSize) =>
            (Element.Arrange(availableSize), null);

        protected override void RenderOverride(DrawingContext context) =>
            Element.Render(context, Size);

        protected override void UpdateElement(DrawingElementBase oldElement, Context oldContext)
        {
            if (!Element.IsArrangeValid(oldElement))
                Context.InvalidateLayout();
            if (!Element.IsRenderValid(oldElement))
                Context.InvalidateRender();
        }
    }
}
