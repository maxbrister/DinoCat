using Microsoft.StandardUI;
using Microsoft.StandardUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pen = Microsoft.StandardUI.Drawing.Pen;
using WpfColor = System.Windows.Media.Color;
using WpfSize = System.Windows.Size;
using WpfRect = System.Windows.Rect;
using WpfPoint = System.Windows.Point;
using WpfBrush = System.Windows.Media.Brush;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfPen = System.Windows.Media.Pen;
using WpfDashStyle = System.Windows.Media.DashStyle;
using WpfDpiScale = System.Windows.DpiScale;
using Microsoft.StandardUI.Elements.Events;
using System.Windows.Input;

namespace Microsoft.StandardUI.Wpf
{
    public static class Extensions
    {
        public static WpfRect Into(this Rect rect) =>
            new WpfRect(rect.Left, rect.Top, rect.Width, rect.Height);
        public static WpfSize Into(this Size size) =>
            new WpfSize(size.Width, size.Height);
        public static Size Into(this WpfSize size) =>
            new Size((float)size.Width, (float)size.Height);
        public static Point Into(this WpfPoint point) =>
            new Point((float)point.X, (float)point.Y);
        public static WpfPoint Into(this Point point) =>
            new WpfPoint(point.X, point.Y);
        public static DpiScale Into(this WpfDpiScale dpi) =>
            new DpiScale((float)dpi.DpiScaleX, (float)dpi.DpiScaleY);
        public static KeyEvent Into(this KeyEventArgs args) =>
            new KeyEvent((Elements.Events.Key)args.Key, args.IsRepeat, (KeyState)args.KeyStates);
    }
}
