using DinoCat.Elements;
using DinoCat.Wpf.Native.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DinoCat.Wpf.Native
{
    public class UIElementBase<TUIElement> : DependencyObjectBase<TUIElement> where TUIElement : UIElement, new()
    {
        public UIElementBase() { }

        public UIElementBase((DependencyProperty, object?)[] localValues, Operation<TUIElement>?[] operations) : base(localValues, operations)
        { }

        public UIElementBase(
            ImmutableDictionary<DependencyProperty, object> localValues,
            ImmutableList<Operation<TUIElement>> operations) : base(localValues, operations)
        { }

        public override Element Build(Context context, DependencyObjectState<TUIElement> state) =>
            new WpfNative<TUIElement>(element => Update(element, state));

        protected static Operation<TUIElement>? MaybeAddHandler(RoutedEvent routedEvent, Action<RoutedEventArgs>? onEvent, bool handledEventsToo = false)
        {
            if (onEvent != null)
                return AddHandler(routedEvent, onEvent, handledEventsToo);
            return null;
        }

        protected static Operation<TUIElement> AddHandler(RoutedEvent routedEvent, Action<RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            void OnEvent(object? sender, RoutedEventArgs args)
            {
                onEvent(args);
            }

            return AddHandler(routedEvent, OnEvent, handledEventsToo);
        }

        protected static Operation<TUIElement> AddHandler(RoutedEvent routedEvent, Action<object, RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            var handler = new RoutedEventHandler(onEvent);
            return new Operation<TUIElement>
            {
                Apply = element => element.AddHandler(routedEvent, handler, handledEventsToo),
                Unapply = element => element.RemoveHandler(routedEvent, handler)
            };
        }
    }
}
