using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public sealed class Paint
    {
        public Paint(Brush? fill = null, Pen? stroke = null)
        {
            Fill = fill;
            Stroke = stroke;
        }

        public static implicit operator Paint(uint c) => new Paint(fill: c);

        public Brush? Fill { get; }
        public Pen? Stroke { get; }
    }
}
