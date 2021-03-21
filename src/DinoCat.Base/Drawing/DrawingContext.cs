using System;
using System.Diagnostics;
using SkiaSharp;

namespace DinoCat.Drawing
{
    public class DrawingContext
    {
        private SKCanvas canvas;

        public DrawingContext(SKCanvas canvas, Matrix deviceTransform)
        {
            this.canvas = canvas;
            DeviceTransform = deviceTransform;
        }

        public Matrix TotalTransform => canvas.TotalMatrix.Into();
        public Matrix DeviceTransform { get; }

        public void DrawRectangle(Paint paint, Rect rect)
        {
            var paints = paint.Into();
            foreach (var p in paints)
            {
                var current = rect.Into();
                if (p.StrokeWidth > 0)
                {
                    var v = p.StrokeWidth * 0.5f;
                    current.Left += v;
                    current.Top += v;
                    current.Right -= v;
                    current.Bottom -= v;
                }
                canvas.DrawRect(current, p);
            }
        }

        public void DrawText(IFormattedText text, Point offset)
        {
            var txt = (FormattedText)text;
            canvas.DrawText(txt.Blob, offset.X, offset.Y - txt.Metrics.Ascent, txt.Paint);
        }

        public IDisposable Push(Matrix transform)
        {
            int saveCount = canvas.Save();

            var t = transform.Into();
            canvas.Concat(ref t);
            return new Pop(canvas, saveCount);
        }

        private record Pop(SKCanvas Canvas, int Count) : IDisposable
        {
            public void Dispose()
            {
                Canvas.Restore();
                Debug.Assert(Canvas.SaveCount == Count, "Unbalanced Push/Pop");
            }
        }
    }
}
