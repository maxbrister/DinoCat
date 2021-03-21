using SkiaSharp;

namespace DinoCat.Drawing
{
    public class FontManager : IFontManager
    {
        private SKFontManager fontManager;
        public FontManager(SKFontManager fontManager) => this.fontManager = fontManager;

        public ITypeface CreateTypeface(string fontName)
        {
            var skTypeface = fontManager.MatchFamily(fontName);
            return new Typeface(skTypeface);
        }

        public ITypeface CreateTypeface(string fontName, FontSlant slant, int weight)
        {
            var skTypeface = fontManager.MatchFamily(fontName, new SKFontStyle((SKFontStyleWeight)weight, SKFontStyleWidth.Normal, (SKFontStyleSlant)slant));
            return new Typeface(skTypeface);
        }

        public IFormattedText FormatText(ITypeface typeface, Brush brush, float fontPt, string text)
        {
            // For some reason SKTextBlob bounds are too wide. Measure separately.
            var paint = brush.Into();
            paint.Typeface = (SKTypeface)typeface.NativeObject;
            paint.TextSize = fontPt;
            var width = paint.MeasureText(text);

            SKFont font = new(paint.Typeface, fontPt);
            font.BaselineSnap = true;
            var metrics = font.Metrics;
            var blob = SKTextBlob.Create(text, font);
            return new FormattedText(blob, metrics, width, text, brush.Into());
        }

        private class Typeface : ITypeface
        {
            private SKTypeface typeface;
            public Typeface(SKTypeface typeface) => this.typeface = typeface;

            public string FamilyName => typeface.FamilyName;

            public FontSlant Slant => throw new System.NotImplementedException();

            public int Weight => throw new System.NotImplementedException();

            public object NativeObject => typeface;
        }
    }
}