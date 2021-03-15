using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public sealed class DashStyle
    {
        public DashStyle(double[] dashes, double offset)
        {
            Dashes = dashes;
            Offset = offset;
        }

        public IReadOnlyList<double> Dashes { get; }
        public double Offset { get; }
    }
}
