using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public abstract class DrawingElementBase : Element
    {
        public override Node CreateNode(int depth, Context context) =>
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
