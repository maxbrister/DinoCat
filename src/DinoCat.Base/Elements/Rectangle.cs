using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Rectangle : DrawingElement<Rectangle>
    {
        public Rectangle(Paint paint, double? width = null, double? height = null)
        {
            Paint = paint;
            Width = width;
            Height = height;
        }

        public Paint Paint { get; }
        public double? Width { get; }
        public double? Height { get; }

        public override Size Arrange(Size availableSize) =>
            new Size(Width ?? availableSize.Width, Height ?? availableSize.Height);

        public override void Render(IDrawingContext context, Size size) =>
            context.DrawRectangle(Paint, new Rect(new Size(Width ?? size.Width, Height ?? size.Height)));

        protected override bool IsArrangeValid(Rectangle oldElement) =>
            Width == oldElement.Width && Height == oldElement.Height;

        protected override bool IsRenderValid(Rectangle oldElement) =>
            oldElement.Paint == Paint;
    }
}
