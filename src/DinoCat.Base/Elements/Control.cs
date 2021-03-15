using DinoCat.Tree;
using System;

namespace DinoCat.Elements
{
    public abstract class Control : Element
    {
        public abstract Element Build(Context context);

        public override Node CreateNode(int depth, Context context) =>
            new ControlNode(depth, context, this);
    }

    public abstract class Control<TState> : Element where TState: IState, new()
    {
        public abstract Element Build(Context context, TState state);

        internal virtual bool Safe { get; } = true;

        public override Node CreateNode(int depth, Context context) =>
            new ControlNode<TState>(depth, context, this);
    }

    public abstract class UnsafeControl<TState> : Control<TState> where TState : IState, new()
    {
        internal override bool Safe => false;
    }
}
