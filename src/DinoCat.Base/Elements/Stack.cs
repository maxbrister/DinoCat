using DinoCat.Drawing;
using DinoCat.Interop;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Elements
{
    public class Stack : Container
    {
        public Stack(params Expand[] children) : base(children.Select(c => c.Child).ToArray())
        {
            Expand = children;
        }

        public IReadOnlyList<Expand> Expand { get; }

        public override Size Arrange(Size availableSize, List<Node> children) =>
            throw new NotImplementedException();

        public override bool IsLayoutInvalid(Container oldContainer) => false;

        public override Node CreateNode(Node? parent, Context context) =>
            new StackNode(parent, context, this);
    }

    internal class StackNode : NodeBase<Stack>
    {
        private List<PseudoLayer> children;

        public StackNode(Node? parent, Context context, Stack stack) : base(parent, context, stack)
        {
            var elementChildren = stack.Children;
            children = new List<PseudoLayer>(elementChildren.Count);

            PseudoLayer? previous = null;
            foreach (var element in elementChildren)
            {
                var child = new PseudoLayer(this, context, element, previous);
                previous = child;
                children.Add(child);
            }
        }

        public override IEnumerable<Node> Children =>
            children.Select(child => child.Node);

        protected override Size ArrangeOverride(Size availableSize)
        {
            var size = new Size();
            for (int i = 0; i < children.Count; ++i)
            {
                if (Element.Expand[i].Flex == 0)
                {
                    var child = children[i];
                    child.Arrange(availableSize);
                    size.Width = Math.Max(size.Width, child.Size.Width);
                    size.Height = Math.Max(size.Height, child.Size.Height);
                }
            }

            for (int i = 0; i < children.Count; ++i)
            {
                var expand = Element.Expand[i];
                var child = children[i];
                if (expand.Flex != 0)
                {
                    child.Arrange(size);
                }

                Aligned.AlignNode(child.Node, size, expand.X, expand.Y);
            }

            return size;
        }

        public override void Dispose()
        {
            foreach (var layer in children)
                layer.Dispose();
        }

        public override IEnumerable<(Node, Point)> HitTest(Point p)
        {
            foreach (var child in Children.Reverse())
                foreach (var hit in child.HitTest(p))
                    yield return hit;
            yield return (this, p);
        }

        protected override void RenderOverride(DrawingContext context)
        {
            foreach (var child in children)
                child.OnRender(context);
        }

        protected override void UpdateElement(Stack oldStack, Context oldContext)
        {
            var newCount = Element.Children.Count;
            if (children.Count < newCount)
            {
                Context.InvalidateLayout();
                var previous = children.LastOrDefault();
                for (int i = children.Count; i < newCount; ++i)
                {
                    var child = new PseudoLayer(this, Context, Element.Children[i], previous);
                    children.Add(child);
                    previous = child;
                }
            }
            else if (children.Count > newCount)
            {
                Context.InvalidateLayout();
                for (int i = newCount; i < children.Count; ++i)
                    children[i].Dispose();
                children.RemoveRange(newCount, children.Count - newCount);
            }

            for (int i = 0; i < newCount; ++i)
                children[i].UpdateElement(Element.Children[i], Context);

        }
    }
}
