using DinoCat.State;
using DinoCat.Tree;
using System;
using System.ComponentModel;

namespace DinoCat.Elements
{
    public abstract class Control : Element
    {
        public abstract Element Build(Context context);

        public override Node CreateNode(Node? parent, Context context) =>
            new ControlNode(parent, context, this);
    }

    public abstract class ControlBase<TState> : Element where TState: INotifyPropertyChanged, new()
    {
        public abstract Element Build(Context context, TState state);

        internal virtual bool Safe { get; } = true;

        public override Node CreateNode(Node? parent, Context context) =>
            new ControlNode<TState>(parent, context, this);
    }

    public abstract class Control<T> : ControlBase<State<T>> where T: new()
    {
        public sealed override Element Build(Context context, State<T> state) =>
            Build(context, state, newState => state.Value = newState);

        public abstract Element Build(Context context, T state, Action<T> setState);
    }

    public abstract class UnsafeControl<TState> : ControlBase<TState> where TState : INotifyPropertyChanged, new()
    {
        internal override bool Safe => false;
    }
}
