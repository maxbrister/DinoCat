using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Tree
{
    public abstract class Node : IDisposable
    {
        public Node(Node? parent, Context context)
        {
            Depth = (parent?.Depth + 1) ?? 0;
            Context = context;
        }

        public Node UpdateElement(Element newElement, Context newContext)
        {
            var oldElement = RawElement;
            if (!Equals(newElement, oldElement))
            {
                if (newElement.GetType() == oldElement.GetType())
                {
                    var oldContext = Context;
                    Context = newContext;
                    SetElement(newElement, oldContext);
                    return this;
                }
                else
                {
                    var context = newContext ?? Context;
                    Dispose();

                    var newNode = newElement.CreateNode(this, context);
                    context.InvalidateLayout();
                    context.InvalidateRender();
                    return newNode;
                }
            }
            else if (newContext != Context)
            {
                var oldContext = Context;
                Context = newContext;
                SetElement(newElement, oldContext);
            }

            return this;
        }

        public abstract IEnumerable<Node> Children { get; }
        // RawElement (instead of Element) because of lack of covariant return types in all target frameworks.
        public abstract Element RawElement { get; }
        public virtual string Description =>
            string.Join(" ", Children.Select(c => c.Description).Where(s => !string.IsNullOrEmpty(s)));
        public int Depth { get; }
        public Context Context { get; private set; }
        public Point Offset { get; set; }
        public Size Size { get; private set; }
        public Rect Bounds
        {
            get => new Rect(Offset, Size);
            set
            {
                Offset = value.Location;
                Size = value.Size;
            }
        }

        public Size Arrange(Size availableSize)
        {
            Size = ArrangeOverride(availableSize);
            return Size;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            foreach (var child in Children)
                child.Dispose();
        }
        public virtual IEnumerable<(Node, Point)> HitTest(Point p)
        {
            foreach (var child in Children.Reverse())
            {
                if (new Rect(child.Offset, child.Size).Contains(p))
                {
                    var childp = p - child.Offset;
                    foreach (var hit in child.HitTest(childp))
                        yield return hit;
                }
            }
            yield return (this, p);
        }
        public void Render(IDrawingContext context)
        {
            if (Offset == new Point())
                RenderOverride(context);
            else
            {
                var trans = Matrix.Translate(Offset);
                using (context.Push(trans))
                    RenderOverride(context);
            }
        }
        protected abstract void SetElement(Element newElement, Context oldContext);
        protected abstract Size ArrangeOverride(Size availableSize);
        protected virtual void RenderOverride(IDrawingContext context)
        {
            foreach (var child in Children)
                child.Render(context);
        }
    }
}
