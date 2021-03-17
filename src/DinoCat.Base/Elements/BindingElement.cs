﻿using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class BindingElement<TState> : Element where TState: IState
    {
        public BindingElement(TState state, Func<Element> child)
        {
            State = state;
            Child = child;
        }

        public TState State { get; }
        public Func<Element> Child { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new BindingNode<TState>(parent, context, this);
    }

    public static class BindingHelper
    {
        public static BindingElement<TState> Binding<TState>(this TState state, Func<Element> child) where TState : IState =>
            new BindingElement<TState>(state, child);
    }

    internal class BindingNode<TState> : NodeBase<BindingElement<TState>> where TState : IState
    {
        private bool disposed;
        private Node child;

        public BindingNode(Node? parent, Context context, BindingElement<TState> binding) : base(parent, context, binding)
        {
            Element.State.StateChanged += State_StateChanged;

            var realChild = binding.Child();
            child = realChild.CreateNode(this, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size availableSize) => child.Arrange(availableSize);

        public override void Dispose()
        {
            base.Dispose();
            disposed = true;
        }

        protected override void UpdateElement(BindingElement<TState> oldElement, Context? oldContext)
        {
            if (!ReferenceEquals(Element.State, oldElement.State))
            {
                oldElement.State.StateChanged -= State_StateChanged;
                Element.State.StateChanged += State_StateChanged;
            }

            UpdateState();
        }

        private void UpdateState()
        {
            if (disposed)
                return;

            var newChild = Element.Child();
            child = child.UpdateElement(newChild, Context);
        }

        private void State_StateChanged(object? sender, EventArgs e) =>
            Context.InvalidateState(Depth, UpdateState);
    }
}
