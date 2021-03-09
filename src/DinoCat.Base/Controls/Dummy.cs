using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Controls
{
    public class Dummy : Element
    {
        public override Size Arrange(IState? state, Size size) => new Size();

        public override IState? CreateOrReuseState(Element? oldElement, IState? oldState) => null;

        public override IEnumerable<BoundElement> GetChildren(IState? state) => Enumerable.Empty<BoundElement>();

        public override IEnumerable<(BoundElement, Point)> Hit(IState? state, Point p) => Enumerable.Empty<(BoundElement, Point)>();

        public override void Render(IState? state, IDrawingContext context)
        { }

        public override void UpdateContext(IState? state, BuildContext oldContext, BuildContext newContext)
        { }

        public override void UpdateState(IState? state, BuildContext context)
        { }
    }
}
