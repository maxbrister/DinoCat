using System;
using System.Collections.Generic;
using SkiaSharp;

namespace DinoCat.Drawing
{
    public static class Extensions
    {
        public static Matrix Into(this SKMatrix matrix) =>
            new Matrix(matrix.ScaleX, matrix.SkewY,
                       matrix.SkewX, matrix.ScaleY,
                       matrix.TransX, matrix.TransY);
        public static SKMatrix Into(this Matrix matrix) =>
            new SKMatrix(matrix.m11, matrix.m21, matrix.m31, matrix.m12, matrix.m22, matrix.m32, 0, 0, 1);

        public static SKPoint Into(this Point point) => new(point.X, point.Y);
        public static SKSize Into(this Size size) => new(size.Width, size.Height);
        public static SKRect Into(this Rect rect) => new(rect.Top, rect.Left, rect.Right, rect.Bottom);

        public static SKColor Into(this Color color) => new SKColor((uint)color);

        public static SKPaint Into(this Brush brush)
        {
            if (brush is SolidColorBrush s)
            {
                return new SKPaint
                {
                    Color = s.Color.Into(),
                    Style = SKPaintStyle.Fill
                };
            }
            else
                throw new Exception("Unrecognized brush");
        }

        public static SKPaint Into(this Pen pen)
        {
            var paint = pen.Brush.Into();
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = pen.Width;
            if (pen.DashStyle is DashStyle dash)
                paint.PathEffect = SKPathEffect.CreateDash(dash.Dashes, dash.Offset);
            return paint;
        }

        public static List<SKPaint> Into(this Paint paint)
        {
            List<SKPaint> paints = new();
            if (paint.Fill is Brush fill)
                paints.Add(fill.Into());

            if (paint.Stroke is Pen stroke)
                paints.Add(stroke.Into());
            return paints;
        }
    }
}