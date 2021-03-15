using DinoCat;
using DinoCat.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using WpfColor = System.Windows.Media.Color;

namespace DinoCat.Wpf
{
    internal class DrawingAdapter
        : IDrawingContext
    {
        private DrawingContext context;
        private List<DinoCat.Matrix> transformStack = new List<DinoCat.Matrix>
        {
            DinoCat.Matrix.Identity
        };

        public DrawingAdapter(DrawingContext context)
        {
            this.context = context;
        }

        public DinoCat.Matrix TotalTransform => transformStack.Last();

        public void DrawRectangle(Paint paint, DinoCat.Rect rect) =>
            context.DrawRectangle(paint.Fill?.Into(), paint.Stroke?.Into(), rect.Into());

        public void DrawText(IFormattedText text, DinoCat.Point offset) =>
            context.DrawText((FormattedText)((DinoFormattedText)text).NativeObject, offset.Into());

        public IDisposable Push(DinoCat.Matrix t)
        {
            transformStack.Add(TotalTransform * t);
            context.PushTransform(new MatrixTransform(t.m11, t.m12, t.m21, t.m22, t.m31, t.m32));
            return new Guard(PopTransform);
        }

        private void PopTransform()
        {
            transformStack.RemoveAt(transformStack.Count - 1);
            context.Pop();
        }
    }
}
