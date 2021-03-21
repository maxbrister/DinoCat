using SkiaSharp;

namespace DinoCat.Drawing
{
    record FormattedText(SKTextBlob Blob, string Text, SKPaint Paint) : IFormattedText
    {
        public float Width => Blob.Bounds.Width;

        public float Height => Blob.Bounds.Height;

        public object NativeObject => Blob;
    }
}