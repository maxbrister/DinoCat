using Microsoft.StandardUI.Elements;
using Microsoft.StandardUI.Wpf.System.Windows.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.StandardUI.Wpf.System.Windows
{
    public partial class UIElementBase<TSubclass, TWpf>
    {
        public override Element Build(Context context, DependencyObjectState<TSubclass, TWpf> state) =>
            new WpfNative<TWpf>(element => Update(element, state));

        public TSubclass On(RoutedEvent routedEvent, Action<RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            var op = AddHandler(routedEvent, onEvent, handledEventsToo);
            return NewImpl(LocalValues, Operations.Add(op));
        }

        public TSubclass On(RoutedEvent routedEvent, Action<object, RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {{
            var op = AddHandler(routedEvent, onEvent, handledEventsToo);
            return NewImpl(LocalValues, Operations.Add(op));
        }}

        protected static Operation<TWpf>? MaybeAddHandler(RoutedEvent routedEvent, Action<RoutedEventArgs>? onEvent, bool handledEventsToo = false)
        {
            if (onEvent != null)
                return AddHandler(routedEvent, onEvent, handledEventsToo);
            return null;
        }

        protected static Operation<TWpf> AddHandler(RoutedEvent routedEvent, Action<RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            void OnEvent(object? sender, RoutedEventArgs args)
            {
                onEvent(args);
            }

            return AddHandler(routedEvent, OnEvent, handledEventsToo);
        }

        protected static Operation<TWpf> AddHandler(RoutedEvent routedEvent, Action<object, RoutedEventArgs> onEvent, bool handledEventsToo = false)
        {
            var handler = new RoutedEventHandler(onEvent);
            return new Operation<TWpf>
            {
                Apply = element => element.AddHandler(routedEvent, handler, handledEventsToo),
                Unapply = element => element.RemoveHandler(routedEvent, handler)
            };
        }
    }
}
