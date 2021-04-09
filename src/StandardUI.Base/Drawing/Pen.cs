using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Drawing
{
    public sealed class Pen
    {
        public Pen(Brush brush, float width = 1, DashStyle? dashStyle = null)
        {
            Brush = brush;
            Width = width;
            DashStyle = dashStyle;
        }

        public static implicit operator Pen(uint c) => new Pen(c);
        public static implicit operator Pen(Brush b) => new Pen(b);

        public Brush Brush { get; }
        public float Width { get; }
        public DashStyle? DashStyle { get; }
    }
}
