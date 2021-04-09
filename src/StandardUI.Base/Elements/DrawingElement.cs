using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Elements
{
    public abstract class DrawingElementBase : Element
    {
        public override Node CreateNode(Node? parent, Context context) =>
            new DrawingNode(parent, context, this);

        public abstract Size Arrange(Size availableSize);
        public abstract void Render(DrawingContext context, Size size);
        public abstract bool IsArrangeValid(DrawingElementBase oldElement);
        public abstract bool IsRenderValid(DrawingElementBase oldElement);
    }

    public abstract class DrawingElement<Self> : DrawingElementBase where Self : DrawingElementBase
    {
        public override bool IsArrangeValid(DrawingElementBase oldElement) =>
            IsArrangeValid((Self)oldElement);
        public override bool IsRenderValid(DrawingElementBase oldElement) =>
            IsRenderValid((Self)oldElement);

        protected abstract bool IsArrangeValid(Self oldElement);
        protected abstract bool IsRenderValid(Self oldElement);
    }
}
