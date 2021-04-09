using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.StandardUI.Wpf.System.Windows.Controls
{
    public partial class Button
    {
        public Button(object? content = null, Action<RoutedEventArgs>? click = null, Brush? background = null, Brush? foreground = null, Thickness? margin = null) : base(
            new[] {
                (ContentProperty, content),
                (BackgroundProperty, background),
                (ForegroundProperty, foreground),
                (MarginProperty, margin) },
            new[] { MaybeAddHandler(ClickEvent, click) })
        {}
    }
}
