using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;

namespace DinoCat.Base.Controls
{
    public abstract class Element
    {
        public event EventHandler<EventArgs>? ArrangeInvalidated;
        public event EventHandler<EventArgs>? RenderInvalidated;

        public abstract IEnumerable<BoundElement> GetChildren(IState? state);

        public abstract Size Arrange(IState? state, Size size);
        /// <summary>
        /// Either create new state, or reuse the previous state if possible.
        /// </summary>
        /// <remarks>
        /// Implementations should not update themselves with the newly created state. Instead, callers
        /// will call UpdateState manually. This gives callers a chance to register for state change notifications
        /// before the state is consumed.
        /// </remarks>
        /// <returns>Valid state for Arrange/Render/Update</returns>
        public abstract IState? CreateOrReuseState(Element? oldElement, IState? oldState);
        public abstract IEnumerable<(BoundElement, Point)> Hit(IState? state, Point p);
        public abstract void Render(IState? state, IDrawingContext context);
        public abstract void UpdateContext(IState? state, BuildContext oldContext, BuildContext newContext);
        public abstract void UpdateState(IState? state, BuildContext context);

        protected void InvalidateArrange() => ArrangeInvalidated?.Invoke(this, EventArgs.Empty);
        protected void InvalidateRender() => RenderInvalidated?.Invoke(this, EventArgs.Empty);
    }
}
