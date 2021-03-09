using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Tree
{
    public class DrawingNode : Node
    {
        private DrawingElementBase element;
        private Size currentSize;

        public DrawingNode(int depth, BuildContext context, DrawingElementBase element) : base(depth, context)
        {
            this.element = element;
        }

        public override IEnumerable<Node> Children =>
            Enumerable.Empty<Node>();

        public override Size Arrange(Size availableSize)
        {
            currentSize = element.Arrange(availableSize);
            return currentSize;
        }

        public override void Dispose() { }

        public override IEnumerable<(Node, Point)> HitTest(Point p)
        {
            if (new Rect(currentSize).Contains(p))
                yield return (this, p);
        }

        public override void Render(IDrawingContext context) =>
            element.Render(context, currentSize);

        public override void UpdateElement(Element newElement)
        {
            var oldElement = element;
            element = (DrawingElementBase)newElement;
            if (!element.IsArrangeValid(oldElement))
                Context.InvalidateLayout();
            if (!element.IsRenderValid(oldElement))
                Context.InvalidateRender();
        }

        public override void UpdateState() { }
    }
}
