using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Controls
{
    public class Rectangle : Element
    {
        private Color color;
        private double? width;
        private double? height;

        public Rectangle(Color color, double? width = null, double? height = null)
        {
            this.color = color;
            this.width = width;
            this.height = height;
        }

        public override Size Arrange(IState? state, Size size) =>
            new Size(width ?? size.Width, height ?? size.Height);

        public override IState? CreateOrReuseState(Element? oldElement, IState? oldState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BoundElement> GetChildren(IState? state)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(BoundElement, Point)> Hit(IState? state, Point p)
        {
            throw new NotImplementedException();
        }

        public override void Render(IState? state, IDrawingContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateContext(IState? state, BuildContext oldContext, BuildContext newContext)
        {
            throw new NotImplementedException();
        }

        public override void UpdateState(IState? state, BuildContext context)
        {
            throw new NotImplementedException();
        }
    }
}
