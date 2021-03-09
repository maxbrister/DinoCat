using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Controls
{
    public abstract class ElementBase<TState> : Element where TState: IState
    {

#pragma warning disable CS8604 // Possible null reference argument.
        public override Size Arrange(IState? state, Size size) =>
            Arrange((TState)state, size);

        public override IEnumerable<BoundElement> GetChildren(IState? state) =>
            GetChildren((TState)state);

        public override IEnumerable<(BoundElement, Point)> Hit(IState? state, Point p) =>
            Hit((TState)state, p);

        public override void Render(IState? state, IDrawingContext context) =>
            Render((TState)state, context);

        public override IState? CreateOrReuseState(Element? oldElement, IState? oldState)
        {
            if (oldElement?.GetType() == GetType() && oldState is TState validState)
            {
                return validState;
            }
            else
            {
                return CreateState();
            }
        }

        public override void UpdateContext(IState? state, BuildContext oldContext, BuildContext newContext)
        {
            UpdateContext((TState)state, oldContext, newContext);
        }

        public override void UpdateState(IState? state, BuildContext context)
        {
            UpdateState((TState)state, context);
        }
#pragma warning restore CS8604 // Possible null reference argument.

        protected abstract Size Arrange(TState state, Size size);
        protected abstract TState CreateState();
        protected abstract IEnumerable<BoundElement> GetChildren(TState state);
        protected abstract IEnumerable<(BoundElement, Point)> Hit(TState state, Point p);
        protected abstract void Render(TState state, IDrawingContext context);
        protected abstract void UpdateContext(TState state, BuildContext oldContext, BuildContext newContext);
        protected abstract void UpdateState(TState state, BuildContext context);
    }
}
