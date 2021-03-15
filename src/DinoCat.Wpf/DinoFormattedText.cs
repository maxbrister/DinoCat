using DinoCat.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Brush = DinoCat.Drawing.Brush;

namespace DinoCat.Wpf
{
    public class DinoFormattedText : IFormattedText
    {
        private FormattedText txt;

        public DinoFormattedText(ITypeface typeface, Brush brush, double fontPt, string text, double dpi) =>
            txt = new FormattedText(text, CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
                (Typeface)((DinoTypeface)typeface).NativeObject, fontPt, brush.Into(), dpi);

        public string Text => txt.Text;

        public double Width => txt.Width;

        public double Height => txt.Height;

        public object NativeObject => txt;
    }
}
