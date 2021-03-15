using DinoCat.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Wpf
{
    public class FontManager : IFontManager
    {
        private double dpi;

        public FontManager(double dpi)
        {
            this.dpi = dpi;
        }

        public ITypeface CreateTypeface(string fontName) =>
            new DinoTypeface(fontName);

        public ITypeface CreateTypeface(string fontName, FontSlant slant, int weight) =>
            new DinoTypeface(fontName, slant, weight);

        public IFormattedText FormatText(ITypeface typeface, Brush brush, double fontPt, string text) =>
            new DinoFormattedText(typeface, brush, fontPt, text, dpi);
    }
}
