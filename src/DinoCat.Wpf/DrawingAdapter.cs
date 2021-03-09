using DinoCat.Base;
using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using WpfColor = System.Windows.Media.Color;

namespace DinoCat.Wpf
{
    internal class DrawingAdapter
        : IDrawingContext
    {
        private DrawingContext context;

        public DrawingAdapter(DrawingContext context) => this.context = context;

        public void DrawRectangle(Paint paint, Rect rect) =>
            context.DrawRectangle(new SolidColorBrush(paint.Foreground.Into()), new Pen(), rect.Into());
    }
}
