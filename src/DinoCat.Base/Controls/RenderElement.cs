using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Controls
{
    public abstract class RenderElement<Self> : Element
    {
        public override Size Arrange(IState? state, Size size) =>
            Arrange(size);

        public override IEnumerable<BoundElement> GetChildren(IState? state) =>
            Children;

        public override IEnumerable<(BoundElement, Point)> Hit(IState? state, Point p) =>
            Hit(p);

        public override IState? CreateOrReuseState(Element? oldElement, IState? oldState)
        {
            return null;
        }

        protected virtual IEnumerable<BoundElement> Children =>
            Enumerable.Empty<BoundElement>();

        protected abstract Size Arrange(Size size);
        protected virtual IEnumerable<(BoundElement, Point)> Hit(Point p) =>
            Enumerable.Empty<(BoundElement, Point)>();
        protected abstract void Render(IDrawingContext context);
        protected virtual void UpdateContext(BuildContext oldContext, BuildContext newContext) { }
        protected abstract void OnElementChanged(Self oldElement);
    }
}
