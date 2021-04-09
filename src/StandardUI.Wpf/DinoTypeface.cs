using Microsoft.StandardUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.StandardUI.Wpf
{
    public class DinoTypeface : ITypeface
    {
        private Typeface typeface;

        public DinoTypeface(Typeface typeface) => this.typeface = typeface;
        public DinoTypeface(string typefaceName) =>
            typeface = new Typeface(typefaceName);

        public DinoTypeface(string familyName, FontSlant slant, int weight)
        {
            FontStyle style;
            switch (slant)
            {
                case FontSlant.Normal:
                    style = FontStyles.Normal;
                    break;
                case FontSlant.Italic:
                    style = FontStyles.Italic;
                    break;
                case FontSlant.Oblique:
                    style = FontStyles.Oblique;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var family = new FontFamily(FamilyName);
            typeface = new Typeface(family, style, FontWeight.FromOpenTypeWeight(weight), FontStretches.Normal);
        }

        public string FamilyName => throw new NotImplementedException();

        public FontSlant Slant => throw new NotImplementedException();

        public int Weight => throw new NotImplementedException();

        public object NativeObject => typeface;
    }
}
