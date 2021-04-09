using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Drawing
{
    public record DpiScale(float X = 1, float Y = 1)
    {
        public DpiScale() : this(1, 1) { }
    }
}
