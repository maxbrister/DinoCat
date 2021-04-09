using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Drawing
{
    public sealed class DashStyle
    {
        public DashStyle(float[] dashes, float offset)
        {
            Dashes = dashes;
            Offset = offset;
        }

        public float[] Dashes { get; }
        public float Offset { get; }
    }
}
