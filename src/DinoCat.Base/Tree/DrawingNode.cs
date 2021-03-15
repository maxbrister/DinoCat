using DinoCat.Drawing;
using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Tree
{
    public class DrawingNode : NodeBase<DrawingElementBase>
    {
        public DrawingNode(int depth, Context context, DrawingElementBase element) : base(depth, context, element) { }

        public override IEnumerable<Node> Children =>
            Enumerable.Empty<Node>();

        protected override Size ArrangeOverride(Size availableSize) =>
            Element.Arrange(availableSize);

        protected override void RenderOverride(IDrawingContext context) =>
            Element.Render(context, Size);

        protected override void UpdateElement(DrawingElementBase oldElement)
        {
            if (!Element.IsArrangeValid(oldElement))
                Context.InvalidateLayout();
            if (!Element.IsRenderValid(oldElement))
                Context.InvalidateRender();
        }

        protected override void UpdateContextOverride(Context oldContext) { }
    }
}
