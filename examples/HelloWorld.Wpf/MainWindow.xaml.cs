using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using System.Windows;

namespace HelloWorld.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            seam.RootElement = () => new Rectangle(Colors.Red, 100, 100);
        }
    }
}
