using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Elements;
using Microsoft.StandardUI.Tree;
using System.Collections.Generic;

namespace Microsoft.StandardUI.Interop
{
    public class PseudoLayer : ILayer, ILayerNode
    {
        Context incommingContext;
        ILayer? real;
        PseudoLayer? next;
        List<ILayer> children = new List<ILayer>();

        public PseudoLayer(Node? parent, Context context, Element childElement,
            PseudoLayer? previous)
        {
            incommingContext = context;
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

        public void UpdateElement(Element newElement, Context newContext)
        {
            Context childContext;
            if (newContext == incommingContext)
                childContext = Node.Context;
            else
            {
                if (newContext.Layer != Parent)
                {
                    ILayer realParent = newContext.Layer;
                    while (realParent is PseudoLayer pseudoParent)
                        realParent = pseudoParent.Parent;

                    if (real != null)
                    {
                        real.Reparent(realParent);
                    }
                    else
                    {
                        foreach (var child in children)
                            child.Reparent(realParent);
                    }
                }

                incommingContext = newContext;
                childContext = new(newContext, this);
            }
            Node = Node.UpdateElement(newElement, childContext);
        }

        public (Size, float?) Arrange(Size availableSize)
        {
            var result = Node.Arrange(availableSize);
            OnArrange(result);
            return result;
        }

        public ILayer AddChild(ILayerNode child)
        {
            ILayer childLayer;
            if (real != null)
                childLayer = real.AddChild(child);
            else
                childLayer = Parent.AddChild(child);
            children.Add(childLayer);
            next?.Realize();
            return childLayer;
        }

        public void Realize()
        {
            if (real != null)
                return;

            real = Parent.AddChild(this);
            foreach (var child in children)
                child.Reparent(real);
        }

        public void Dispose()
        {
            Node.Dispose();
            real?.Dispose();
        }

        public void Focus() => (real ?? Parent).Focus();

        public void FocusNext() => (real ?? Parent).FocusNext();

        public void FocusPrevious() => (real ?? Parent).FocusPrevious();

        public void InvalidateLayout()
        {
            if (real != null)
                real.InvalidateLayout();
            else
                Parent.InvalidateLayout();
        }

        public void InvalidateRender()
        {
            if (real != null)
                real.InvalidateRender();
            else
                Parent.InvalidateRender();
        }

        public void OnArrange((Size, float?) result)
        {
            real?.OnArrange(result);
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

        ILayer Parent => incommingContext.Layer;
    }
}
