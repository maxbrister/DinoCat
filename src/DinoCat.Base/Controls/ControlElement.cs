using DinoCat.Base.Controls;
using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Base.Controls
{
    internal class ControlElementState : State
    {
        private IState? childState;
        internal ControlElementState(Control control)
        {
            Control = control;
        }

        internal IState? ChildState
        {
            get => childState;
            set
            {
                if (childState != null)
                {
                    childState.StateChanged -= ChildState_StateChanged;
                }

                childState = value;

                if (childState != null)
                {
                    childState.StateChanged += ChildState_StateChanged;
                }
            }
        }

        internal Control Control;
        internal Element? Child;

        internal BoundElement Bound => new BoundElement(Child!, ChildState);

        private void ChildState_StateChanged(object? sender, EventArgs e) =>
            Notify();
    }

    internal class ControlElement : ElementBase<ControlElementState>
    {
        private Control control;

        public ControlElement(Control control)
        {
            this.control = control;
        }

        protected override Size Arrange(ControlElementState state, Size size) =>
            state.Bound.Arrange(size);

        protected override ControlElementState CreateState() =>
            new ControlElementState(control);

        protected override IEnumerable<BoundElement> GetChildren(ControlElementState state) =>
            state.Bound.Children;

        protected override IEnumerable<(BoundElement, Point)> Hit(ControlElementState state, Point p) =>
            state.Bound.Hit(p);

        protected override void Render(ControlElementState state, IDrawingContext context) =>
            state.Bound.Render(context);

        protected override void UpdateContext(ControlElementState state, BuildContext oldContext, BuildContext newContext)
        {
            state.Child = control.Build(newContext);
            state.ChildState = state.Bound.CreateOrReuseState(newContext);
            state.Bound.UpdateContext(oldContext, newContext);
        }

        protected override void UpdateState(ControlElementState state, BuildContext context)
        {
            if (!Equals(control, state.Control))
            {
                state.Control = control;
                state.Child = control.Build(context);
                state.ChildState = state.Bound.CreateOrReuseState(context);
            }

            state.Bound.UpdateState(context);
        }
    }

    internal class ControlElementState<TState> : State where TState: IState, new()
    {
        private IState? childState;

        public ControlElementState(Control<TState> control, BuildContext context)
        {
            Control = control;
            ControlState.StateChanged += ControlState_StateChanged;
            Child = control.Build(context, ControlState);
        }

        public BoundElement Bound => new BoundElement(Child, ChildState);
        public Control<TState> Control { get; set; }
        public Element Child { get; set; }
        public TState ControlState { get; } = new TState();
        public bool ControlStateValid { get; set; } = true;
        public IState? ChildState
        {
            get => childState;
            set
            {
                if (childState != value)
                {
                    if (childState != null)
                        childState.StateChanged -= ChildState_StateChanged;

                    childState = value;

                    if (childState != null)
                        childState.StateChanged += ChildState_StateChanged;
                }
            }
        }

        private void ControlState_StateChanged(object? sender, EventArgs e)
        {
            if (ControlStateValid)
            {
                ControlStateValid = false;
                Notify();
            }
        }

        private void ChildState_StateChanged(object? sender, EventArgs e) =>
            Notify();
    }

    internal class ControlElement<TState> : ElementBase<ControlElementState<TState>> where TState: IState, new()
    {
        private Control<TState> control;

        public ControlElement(Control<TState> control) => this.control = control;

        protected override Size Arrange(ControlElementState<TState> state, Size size) =>
            state.Bound.Arrange(size);

        protected override ControlElementState<TState> CreateState()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<BoundElement> GetChildren(ControlElementState<TState> state) =>
            state.Bound.Children;

        protected override IEnumerable<(BoundElement, Point)> Hit(ControlElementState<TState> state, Point p) =>
            state.Bound.Hit(p);

        protected override void Render(ControlElementState<TState> state, IDrawingContext context) =>
            state.Bound.Render(context);

        protected override void UpdateContext(ControlElementState<TState> state, BuildContext oldContext, BuildContext newContext)
        {
            state.ControlStateValid = true;
            state.Child = control.Build(newContext, state.ControlState);
            state.ChildState = state.Bound.CreateOrReuseState(newContext);
            state.Bound.UpdateContext(oldContext, newContext);
        }

        protected override void UpdateState(ControlElementState<TState> state, BuildContext context)
        {
            if (!state.ControlStateValid || !Equals(control, state.Control))
            {
                state.Control = control;
                state.ControlStateValid = true;
                state.Child = control.Build(context, state.ControlState);
                state.ChildState = state.Bound.CreateOrReuseState(context);
            }

            state.Bound.UpdateState(context);
        }
    }
}
