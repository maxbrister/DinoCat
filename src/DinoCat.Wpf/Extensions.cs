using DinoCat.Base;
using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WpfColor = System.Windows.Media.Color;
using WpfSize = System.Windows.Size;
using WpfRect = System.Windows.Rect;

namespace DinoCat.Wpf
{
    public static class Extensions
    {
        public static WpfColor Into(this Color color) =>
            WpfColor.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        public static WpfRect Into(this Rect rect) =>
            new WpfRect(rect.Left, rect.Top, rect.Width, rect.Height);
        public static WpfSize Into(this Size size) =>
            new WpfSize(size.Width, size.Height);
        public static Size Into(this WpfSize size) =>
            new Size(size.Width, size.Height);
    }
}
