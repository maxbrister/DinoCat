using DinoCat.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interop.Wpf
{
    /// <summary>
    /// Interaction logic for MyUserControl.xaml
    /// </summary>
    [FromWpfType("Interop.Wpf.MyUserControlWrapper")]
    public partial class MyUserControl : UserControl
    {
        public static readonly DependencyProperty MyValueProperty = DependencyProperty.Register(nameof(MyValue), typeof(int), typeof(MyUserControl), new PropertyMetadata(0));
        private static int nextId;

        public MyUserControl()
        {
            Id = nextId++;
            InitializeComponent();
        }

        public int MyValue
        {
            get => (int)GetValue(MyValueProperty);
            set => SetValue(MyValueProperty, value);
        }

        public int Id { get; set; }
    }
}
