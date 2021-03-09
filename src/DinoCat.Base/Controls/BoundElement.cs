using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Controls
{
    public struct BoundElement
    {
        public BoundElement(Element element, IState? state)
        {
            Element = element;
            State = state;
        }

        public Element Element;
        public IState? State;



        public IEnumerable<BoundElement> Children => Element.GetChildren(State);
        public IState? CreateOrReuseState(BuildContext context) => throw new NotImplementedException();
        public Size Arrange(Size size) => Element.Arrange(State, size);
        public IEnumerable<(BoundElement, Point)> Hit(Point p) => Element.Hit(State, p);
        public void Render(IDrawingContext context) => Element.Render(State, context);
        public void UpdateContext(BuildContext oldContext, BuildContext newContext) => Element.UpdateContext(State, oldContext, newContext);
        public void UpdateState(BuildContext context) => Element.UpdateState(State, context);
    }
}
