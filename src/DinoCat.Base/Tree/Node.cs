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
        public Node(int parentDepth, Context context)
        {
            Depth = parentDepth + 1;
            Context = context;
        }

        public Node UpdateElement(Element newElement, Context? newContext = null)
        {
            var oldElement = RawElement;
            if (!Equals(newElement, oldElement))
            {
                if (newElement.GetType() == oldElement.GetType())
                {
                    SetElement(newElement);
                    if (newContext != null)
                        UpdateContext(newContext);
                    return this;
                }
                else
                {
                    var depth = Depth;
                    var context = newContext ?? Context;
                    Dispose();

                    var newNode = newElement.CreateNode(depth - 1, context);
                    context.InvalidateLayout();
                    context.InvalidateRender();
                    return newNode;
                }
            }

            if (newContext != null)
                UpdateContext(newContext);

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
        public void UpdateContext(Context newContext)
        {
            var oldContext = Context;
            Context = newContext;
            UpdateContextOverride(oldContext);
        }
        public abstract void SetElement(Element newElement);

        protected abstract Size ArrangeOverride(Size availableSize);
        protected abstract void UpdateContextOverride(Context oldContext);
        protected virtual void RenderOverride(IDrawingContext context)
        {
            foreach (var child in Children)
                child.Render(context);
        }
    }
}
