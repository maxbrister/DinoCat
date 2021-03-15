using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public abstract class Brush
    {
        internal Brush() { }

        public static implicit operator Brush(uint c) => new SolidColorBrush(c);
    }
}
