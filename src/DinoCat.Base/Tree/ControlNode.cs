using DinoCat.Drawing;
using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DinoCat.Tree
{
    internal sealed class ControlNode : NodeBase<Control>
    {
        private Node child;

        public ControlNode(int parentDepth, Context context, Control control) : base(parentDepth, context, control)
        {
            var childElement = control.Build(context);
            child = childElement.CreateNode(parentDepth, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size availableSize) => child.Arrange(availableSize);

        protected override void UpdateElement(Control oldElement)
        {
            if (!Equals(Element, oldElement))
            {
                var newChild = Element.Build(Context);
                child = child.UpdateElement(newChild);
            }
        }

        protected override void UpdateContextOverride(Context oldContext)
        {
            var newChild = Element.Build(Context);
            child = child.UpdateElement(newChild, Context);
        }
    }

    internal sealed class ControlNode<TState> : NodeBase<Control<TState>> where TState : IState, new()
    {
        private TState state;
        private Node child;
        private bool disposed;
        private long modificationCount;
        private long lastUpdate;

        public ControlNode(int parentDepth, Context context, Control<TState> control) : base(parentDepth, context, control)
        {
            state = new();

            if (control.Safe)
                state.StateChanged += State_StateChanged;

            var childElement = control.Build(context, state);
            child = childElement.CreateNode(parentDepth, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size availableSize) =>
            child.Arrange(availableSize);

        public override void Dispose()
        {
            disposed = true;
            state.StateChanged -= State_StateChanged;
            child.Dispose();
            state.Dispose();
        }

        protected override void UpdateElement(Control<TState> oldElement)
        {
            if (!Equals(oldElement, Element))
            {
                var newChild = Element.Build(Context, state);
                child = child.UpdateElement(newChild);
            }
        }

        protected override void UpdateContextOverride(Context oldContext)
        {
            var newChild = Element.Build(Context, state);
            child = child.UpdateElement(newChild, Context);
        }

        private void UpdateState()
        {
            if (disposed || modificationCount == lastUpdate)
                return;

            lastUpdate = modificationCount;

            var newChild = Element.Build(Context, state);
            child = child.UpdateElement(newChild);
        }

        private void State_StateChanged(object? sender, EventArgs e)
        {
            if (modificationCount == lastUpdate)
            {
                Interlocked.Increment(ref modificationCount);
                Context.InvalidateState(Depth, UpdateState);
            }
        }
    }
}
