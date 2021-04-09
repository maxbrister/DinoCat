using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public static class DashStyles
    {
        public static DashStyle Dash { get; } = new DashStyle(new[] { 2.0f, 2.0f }, 1);
        public static DashStyle Dot { get; } = new DashStyle(new[] { 0.0f, 2.0f }, 0);
    }
}
