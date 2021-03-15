using System;

namespace DinoCat.Drawing
{
    public interface IDrawingContext
    {
        Matrix TotalTransform { get; }
        void DrawRectangle(Paint paint, Rect rect);
        void DrawText(IFormattedText text, Point offset);
        IDisposable Push(Matrix transform);
    }
}
