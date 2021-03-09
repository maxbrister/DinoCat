using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;

namespace DinoCat.Base.Tree
{
    public abstract class Node : IDisposable
    {
        public Node(int depth, BuildContext context)
        {
            Depth = depth;
            Context = context;
        }

        public abstract IEnumerable<Node> Children { get; }
        public int Depth { get; }
        public BuildContext Context { get; private set; }

        public abstract Size Arrange(Size availableSize);
        public abstract void Dispose();
        public abstract IEnumerable<(Node, Point)> HitTest(Point p);
        public abstract void Render(IDrawingContext context);
        public virtual void UpdateContext(BuildContext newContext) =>
            this.Context = newContext;
        public abstract void UpdateElement(Element newElement);
        public abstract void UpdateState();
    }
}
