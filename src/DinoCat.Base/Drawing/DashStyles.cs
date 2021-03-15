using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public static class DashStyles
    {
        public static DashStyle Dash { get; } = new DashStyle(new double[] { 2, 2 }, 1);
        public static DashStyle Dot { get; } = new DashStyle(new double[] { 0, 2 }, 0);
    }
}
