using SkiaSharp;

namespace DinoCat.Drawing
{
    record FormattedText(SKTextBlob Blob, SKFontMetrics Metrics, float Width, string Text, SKPaint Paint) : IFormattedText
    {
        public float Height => Metrics.Descent - Metrics.Ascent;

        public object NativeObject => Blob;
    }
}