using DinoCat.State;
using DinoCat.Wpf.Native.Internal;
using System;
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Media;
using WpfButton = System.Windows.Controls.Button;

namespace DinoCat.Wpf.Native
{
    public class Button : UIElementBase<WpfButton>
    {
        public static DependencyProperty ContentProperty = WpfButton.ContentProperty;

        public Button() { }

        public Button(object? content = null, Action<RoutedEventArgs>? click = null, Brush? background = null, Brush? foreground = null, Thickness? margin = null) : base(
            new[] {
                (WpfButton.ContentProperty, content),
                (WpfButton.BackgroundProperty, background),
                (WpfButton.ForegroundProperty, foreground),
                (WpfButton.MarginProperty, margin) },
            new[] { MaybeAddHandler(WpfButton.ClickEvent, click) })
        {
        }

        public Button(
            ImmutableDictionary<DependencyProperty, object> localValues,
            ImmutableList<Operation<WpfButton>> operations) : base(localValues, operations)
        { }

        public Button Set(DependencyProperty dp, object v) =>
            new Button(LocalValues.Add(dp, v), Operations);

        public Button Set(DependencyProperty dp, Func<object> v) =>
            new Button(LocalValues.Add(dp, new ImplicitStateScope<object>(v)), Operations);

        public Button On(RoutedEvent routedEvent, Action<RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            var op = AddHandler(routedEvent, onEvent, handledEventsToo);
            return new Button(LocalValues, Operations.Add(op));
        }

        public Button On(RoutedEvent routedEvent, Action<object, RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            var op = AddHandler(routedEvent, onEvent, handledEventsToo);
            return new Button(LocalValues, Operations.Add(op));
        }
    }
}
