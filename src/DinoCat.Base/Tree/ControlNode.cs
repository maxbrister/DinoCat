using DinoCat.Base.Controls;
using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DinoCat.Base.Tree
{
    internal sealed class ControlNode : Node
    {
        private Control control;
        private Site child;

        public ControlNode(int depth, BuildContext context, Control control) : base(depth, context)
        {
            this.control = control;
            var childElement = control.Build(context);
            child = new Site(depth, context, childElement);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        public override Size Arrange(Size availableSize) => child.Arrange(availableSize);

        public override void Dispose() => child.Dispose();

        public override IEnumerable<(Node, Point)> HitTest(Point p)
        {
            foreach (var hit in child.HitTest(p))
                yield return hit;
            yield return (this, p);
        }

        public override void Render(IDrawingContext context) => child.Render(context);

        public override void UpdateElement(Element newElement)
        {
            var newControl = (Control)newElement;
            if (!Equals(newControl, control))
            {
                control = newControl;
                var newChild = control.Build(Context);
                child.UpdateElement(newChild);
            }
        }

        public override void UpdateState() { }
    }

    internal sealed class ControlNode<TState> : Node where TState : IState, new()
    {
        private Control<TState> control;
        private TState state;
        private Site child;
        private bool disposed;
        private long modificationCount;
        private long lastUpdate;

        public ControlNode(int depth, BuildContext context, Control<TState> control) : base(depth, context)
        {
            state = new TState();
            state.StateChanged += State_StateChanged;

            this.control = control;
            var childElement = control.Build(context, state);
            child = new Site(depth, context, childElement);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        public override Size Arrange(Size availableSize) =>
            child.Arrange(availableSize);

        public override void Dispose()
        {
            disposed = true;
            state.StateChanged -= State_StateChanged;
            state.Dispose();
            child.Dispose();
        }

        public override IEnumerable<(Node, Point)> HitTest(Point p)
        {
            foreach (var hit in child.HitTest(p))
                yield return hit;
            yield return (this, p);
        }

        public override void Render(IDrawingContext context) =>
            child.Render(context);

        public override void UpdateElement(Element newElement)
        {
            var newControl = (Control<TState>)newElement;
            if (!Equals(newControl, control))
            {
                control = newControl;

                var newChild = control.Build(Context, state);
                child.UpdateElement(newChild);
            }
        }

        public override void UpdateState()
        {
            if (disposed || modificationCount == lastUpdate)
                return;

            lastUpdate = modificationCount;

            var newChild = control.Build(Context, state);
            child.UpdateElement(newChild);
        }

        private void State_StateChanged(object? sender, EventArgs e)
        {
            if (modificationCount == lastUpdate)
            {
                Interlocked.Increment(ref modificationCount);
                Context.InvalidateState(this);
            }
        }
    }
}
