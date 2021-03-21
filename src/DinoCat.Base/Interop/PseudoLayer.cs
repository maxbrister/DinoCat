using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Tree;
using System.Collections.Generic;

namespace DinoCat.Interop
{
    public class PseudoLayer : ILayer, ILayerNode
    {
        private ILayer parent;
        private ILayer? real;
        private PseudoLayer? next;
        private List<ILayer> children = new List<ILayer>();

        public PseudoLayer(Node? parent, Context context, Element childElement,
            PseudoLayer? previous)
        {
            this.parent = context.Layer;
            var childContext = new Context(context, this);
            if (previous != null)
                previous.Next = this;
            Node = childElement.CreateNode(parent, childContext);
        }

        public Node Node { get; private set; }

        public PseudoLayer? Next
        {
            get => next;
            set
            {
                next = value;

                if (children.Count > 0)
                    next?.Realize();
            }
        }

        public Size Size => Node.Size;

        public void UpdateElement(Element newElement, Context newContext) => Node = Node.UpdateElement(newElement, newContext);

        public void UpdateContext(Element newElement, Context newContext)
        {
            if (newContext.Layer != parent)
            {
                parent = newContext.Layer;
                foreach (var child in children)
                    child.Reparent(parent);
            }

            var childContext = new Context(newContext, this);
            Node = Node.UpdateElement(newElement, childContext);
        }

        public Size Arrange(Size availableSize)
        {
            var size = Node.Arrange(availableSize);
            OnArrange(size);
            return size;
        }

        public ILayer AddChild(ILayerNode child)
        {
            ILayer childLayer;
            if (real != null)
                childLayer = real.AddChild(child);
            else
                childLayer = parent.AddChild(child);
            children.Add(childLayer);
            next?.Realize();
            return childLayer;
        }

        public void Realize()
        {
            if (real != null)
                return;

            real = parent.AddChild(this);
            foreach (var child in children)
                child.Reparent(real);
        }

        public void Dispose()
        {
            Node.Dispose();
            real?.Dispose();
        }

        public void Focus() => (real ?? parent).Focus();

        public void FocusNext() => (real ?? parent).FocusNext();

        public void FocusPrevious() => (real ?? parent).FocusPrevious();

        public void InvalidateLayout()
        {
            if (real != null)
                real.InvalidateLayout();
            else
                parent.InvalidateLayout();
        }

        public void InvalidateRender()
        {
            if (real != null)
                real.InvalidateRender();
            else
                parent.InvalidateRender();
        }

        public void OnArrange(Size size)
        {
            real?.OnArrange(size);
        }

        public void OnRender(DrawingContext drawingContext)
        {
            if (real != null)
                real?.OnRender(drawingContext);
            else
                Node.Render(drawingContext);
        }

        public void OnUpdated() => real?.OnUpdated();

        public void Reparent(ILayer newParent)
        {
            if (real != null)
                real.Reparent(newParent);
            else
            {
                foreach (var child in children)
                    child.Reparent(newParent);
            }
        }

        void ILayerNode.RenderLayer(DrawingContext context) =>
            Node.Render(context);
    }
}
