using DinoCat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfButton = System.Windows.Controls.Button;

namespace DinoCat.Wpf.Native
{
    public class Button : Elements.Control
    {
        private static ConditionalWeakTable<WpfButton, Button> stash = new();

        public Button(object content, Action<RoutedEventArgs>? click)
        {
            Content = content;
            Click = click;
        }

        public object Content { get; }
        public Action<RoutedEventArgs>? Click { get; }

        public override Elements.Element Build(Context context) =>
            new WpfNative<WpfButton>(btn =>
            {
                stash.Remove(btn);
                stash.Add(btn, this);

                btn.Content = Content;
                btn.Click -= Btn_Click;
                if (Click != null)
                    btn.Click += Btn_Click;
            });

        private static void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is WpfButton wpfBtn && stash.TryGetValue(wpfBtn, out var btn))
                btn.Click?.Invoke(e);
        }

    }
}
