using DinoCat.Base.Drawing;
using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Elements
{
    public abstract class DrawingElementBase : Element
    {
        public Node CreateNode(int depth, BuildContext context) =>
            new DrawingNode(depth, context, this);

        public abstract Size Arrange(Size availableSize);
        public abstract void Render(IDrawingContext context, Size size);
        public abstract bool IsArrangeValid(DrawingElementBase oldElement);
        public abstract bool IsRenderValid(DrawingElementBase oldElement);
    }

    public abstract class DrawingElement<Self> : DrawingElementBase where Self: DrawingElementBase
    {
        public override bool IsArrangeValid(DrawingElementBase oldElement) =>
            IsArrangeValid((Self)oldElement);
        public override bool IsRenderValid(DrawingElementBase oldElement) =>
            IsRenderValid((Self)oldElement);

        protected abstract bool IsArrangeValid(Self oldElement);
        protected abstract bool IsRenderValid(Self oldElement);
    }
}
