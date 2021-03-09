using System;

namespace DinoCat.Base.Controls
{
    public abstract class Control
    {
        public static implicit operator Element(Control c) => new ControlElement(c);

        public abstract Element Build(BuildContext context);
    }

    public abstract class Control<TState> where TState: IState, new()
    {
        public static implicit operator Element(Control<TState> c) => new ControlElement<TState>(c);
        public abstract Element Build(BuildContext context, TState state);
    }
}
