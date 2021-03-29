using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace DinoCat.Wpf
{
    [ContentProperty("Child")]
    public class DinoExpand : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(nameof(Child), typeof(UIElement), typeof(DinoExpand));
        public static readonly DependencyProperty FlexProperty = DependencyProperty.Register(nameof(Flex), typeof(int), typeof(DinoExpand));
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(float), typeof(DinoExpand));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(float), typeof(DinoExpand));
        public event PropertyChangedEventHandler? PropertyChanged;

        public Expand Expand => new(new ContentAdapter(Child), Flex, X, Y);

        public UIElement Child
        {
            get => (UIElement)GetValue(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        public int Flex
        {
            get => (int)GetValue(FlexProperty);
            set => SetValue(FlexProperty, value);
        }

        public float? X
        {
            get => (float?)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public float? Y
        {
            get => (float?)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Expand)));
        }
    }

    public class ExpandConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(UIElement);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is UIElement uie)
                return new DinoExpand() { Child = uie };
            throw new ArgumentException("Unable to convert values of the given type", nameof(value));
        }
    }
}
