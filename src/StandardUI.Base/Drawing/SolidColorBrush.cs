using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public sealed class SolidColorBrush : Brush
    {
        public SolidColorBrush(Color color) => Color = color;

        public Color Color { get; }
    }
}
