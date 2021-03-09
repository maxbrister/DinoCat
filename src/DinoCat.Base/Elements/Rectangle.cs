using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Elements
{
    public class Rectangle : DrawingElement<Rectangle>
    {
        public Rectangle(Color color, double? width = null, double? height = null)
        {
            Color = color;
            Width = width;
            Height = height;
        }

        public Color Color { get; }
        public double? Width { get; }
        public double? Height { get; }

        public override Size Arrange(Size availableSize) =>
            new Size(Width ?? availableSize.Width, Height ?? availableSize.Height);

        public override void Render(IDrawingContext context, Size size) =>
            context.DrawRectangle(new Paint
            {
                Foreground = Color
            }, new Rect(size));

        protected override bool IsArrangeValid(Rectangle oldElement) =>
            Width == oldElement.Width && Height == oldElement.Height;

        protected override bool IsRenderValid(Rectangle oldElement) =>
            oldElement.Color == Color;
    }
}
