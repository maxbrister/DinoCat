using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using System;
using System.Collections.Generic;

namespace DinoCat.Base.Tree
{
    public struct Site : IDisposable
    {
        private Element element;

        public Site(int parentDepth, BuildContext context, Element element)
        {
            this.element = element;
            Node = element.CreateNode(parentDepth + 1, context);
        }

        public static implicit operator Node(Site site) => site.Node;

        public Node Node { get; private set; }

        public IEnumerable<Node> Children => Node.Children;
        public Size Arrange(Size availableSize) => Node.Arrange(availableSize);
        public void Dispose() => Node.Dispose();
        public IEnumerable<(Node, Point)> HitTest(Point p) => Node.HitTest(p);
        public void Render(IDrawingContext context) => Node.Render(context);
        public void UpdateContext(Element newElement, BuildContext newContext)
        {
            if (!UpdateElement(newElement))
                Node.UpdateContext(newContext);
        }
        public bool UpdateElement(Element newElement)
        {
            if (!Equals(newElement, element))
            {
                if (newElement.GetType() == element.GetType())
                {
                    element = newElement;
                    Node.UpdateElement(newElement);
                    return false;
                }
                else
                {
                    int depth = Node.Depth;
                    BuildContext context = Node.Context;
                    Node.Dispose();

                    element = newElement;
                    Node = element.CreateNode(depth, context);
                    context.InvalidateLayout();
                    context.InvalidateRender();
                    return true;
                }
            }

            return false;
        }
        public void UpdateState() => Node.UpdateState();
    }
}
