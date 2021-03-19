using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DinoCat.Wpf
{
    public class DinoButton : Host
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DinoButton),
            new PropertyMetadata("", DinoButton.OnTextChanged));

        public event EventHandler<EventArgs>? Clicked;

        public DinoButton()
        {
            RootElement = () => new Elements.Button(() => Clicked?.Invoke(this, EventArgs.Empty), Text);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((DinoButton)sender).root.Refresh();
        }
    }
}
