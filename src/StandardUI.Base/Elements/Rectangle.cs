using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Elements;
using Microsoft.StandardUI.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Elements
{
    public class Rectangle : DrawingElement<Rectangle>
    {
        public Rectangle(Paint paint, float? width = null, float? height = null)
        {
            Paint = paint;
            Width = width;
            Height = height;
        }

        public Paint Paint { get; }
        public float? Width { get; }
        public float? Height { get; }

        public override Size Arrange(Size availableSize) =>
            new Size(Width ?? availableSize.Width, Height ?? availableSize.Height);

        public override void Render(DrawingContext context, Size size) =>
            context.DrawRectangle(Paint, new Rect(new Size(Width ?? size.Width, Height ?? size.Height)));

        protected override bool IsArrangeValid(Rectangle oldElement) =>
            Width == oldElement.Width && Height == oldElement.Height;

        protected override bool IsRenderValid(Rectangle oldElement) =>
            oldElement.Paint == Paint;
    }
}
