using DinoCat.Drawing;
using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace DinoCat.Tree
{
    internal sealed class ControlNode : NodeBase<Control>
    {
        private Node child;

        public ControlNode(Node? parent, Context context, Control control) : base(parent, context, control)
        {
            var childElement = control.Build(context);
            child = childElement.CreateNode(this, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size availableSize) => child.Arrange(availableSize);

        protected override void UpdateElement(Control oldElement, Context oldContext)
        {
            if (!Equals(Element, oldElement))
            {
                var newChild = Element.Build(Context);
                child = child.UpdateElement(newChild, Context);
            }
            else if (Context != oldContext)
                child = child.UpdateElement(child.RawElement, Context);
        }
    }

    internal sealed class ControlNode<TState> : NodeBase<ControlBase<TState>> where TState : INotifyPropertyChanged, new()
    {
        private TState state;
        private Node child;
        private bool disposed;
        private long modificationCount;
        private long lastUpdate;

        public ControlNode(Node? parent, Context context, ControlBase<TState> control) : base(parent, context, control)
        {
            state = new();

            if (control.Safe)
                state.PropertyChanged += State_PropertyChanged;

            var childElement = control.Build(context, state);
            child = childElement.CreateNode(this, context);
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
            state.PropertyChanged -= State_PropertyChanged;
            child.Dispose();
            (state as IDisposable)?.Dispose();
        }

        protected override void UpdateElement(ControlBase<TState> oldElement, Context oldContext)
        {
            if (!Equals(oldElement, Element))
            {
                var newChild = Element.Build(Context, state);
                child = child.UpdateElement(newChild, Context);
            }
            else if (oldContext != Context)
                child = child.UpdateElement(child.RawElement, Context);
        }

        private void UpdateState()
        {
            if (disposed || modificationCount == lastUpdate)
                return;

            lastUpdate = modificationCount;

            var newChild = Element.Build(Context, state);
            child = child.UpdateElement(newChild, Context);
        }

        private void State_PropertyChanged(object? sender, EventArgs e)
        {
            if (modificationCount == lastUpdate)
            {
                Interlocked.Increment(ref modificationCount);
                Context.InvalidateState(Depth, UpdateState);
            }
        }
    }
}
