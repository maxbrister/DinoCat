using DinoCat.State;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static DinoCat.Elements.Factories;

namespace DinoCat.Elements
{
    public abstract class BindingElementBase : Element, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BindingElementBase(INotifyPropertyChanged basedOn)
        {
            BasedOn = basedOn;
            basedOn.PropertyChanged += BasedOn_PropertyChanged;
        }

        public INotifyPropertyChanged BasedOn { get; }

        public abstract object Value { get; }

        public Element EvaluateAsElement()
        {
            var obj = Value;
            if (obj is Element e)
                return e;
            return Text(obj.ToString() ?? "");
        }

        private void BasedOn_PropertyChanged(object? sender, PropertyChangedEventArgs e) =>
            PropertyChanged?.Invoke(this, PropertyChangedHelper.All);
    }

    public class BindingElement<TState, TOutput> : BindingElementBase where TState: INotifyPropertyChanged where TOutput : notnull
    {
        public BindingElement(TState state, Func<TOutput> child) : base(state)
        {
            Child = child;
        }

        public Func<TOutput> Child { get; }
        public override object Value => Child();

        public override Node CreateNode(Node? parent, Context context) =>
            new BindingNode<TState>(parent, context, this);
    }

    public static class BindingHelper
    {
        public static BindingElement<TState, TOutput> Bind<TState, TOutput>(this TState state, Func<TOutput> child)
            where TState : INotifyPropertyChanged
            where TOutput : notnull =>
            new BindingElement<TState, TOutput>(state, child);
    }

    internal class BindingNode<TState> : NodeBase<BindingElementBase>
    {
        private bool disposed;
        private Node child;

        public BindingNode(Node? parent, Context context, BindingElementBase binding) : base(parent, context, binding)
        {
            binding.PropertyChanged += State_PropertyChanged;

            var element = binding.EvaluateAsElement();
            child = element.CreateNode(this, context);
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

        protected override void UpdateElement(BindingElementBase oldElement, Context? oldContext)
        {
            if (!ReferenceEquals(Element.BasedOn, oldElement.BasedOn))
            {
                oldElement.BasedOn.PropertyChanged -= State_PropertyChanged;
                Element.BasedOn.PropertyChanged += State_PropertyChanged;
            }

            UpdateState();
        }

        private void UpdateState()
        {
            if (disposed)
                return;

            var newChild = Element.EvaluateAsElement();
            child = child.UpdateElement(newChild, Context);
        }

        private void State_PropertyChanged(object? sender, EventArgs e) =>
            Context.InvalidateState(Depth, UpdateState);
    }
}
