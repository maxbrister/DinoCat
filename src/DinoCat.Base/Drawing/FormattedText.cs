using SkiaSharp;

namespace DinoCat.Drawing
{
    record FormattedText(SKTextBlob Blob, float Width, float Height, float Ascent, string Text, SKPaint Paint) : IFormattedText
    {
        public object NativeObject => Blob;
    }
}