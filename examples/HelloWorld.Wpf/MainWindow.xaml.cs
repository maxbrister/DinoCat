using DinoCat;
using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Wpf;
using System.Diagnostics;
using System.Windows;

using WpfButton = DinoCat.Wpf.Native.Button;

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

            host.RootElement = () => State.Inject<int>(state =>
                new Row(
                    new Stack(
                        new WpfButton($"Hello world {state}", click: _ => ++state.Value)
                            .PadUniform(10)
                        // ,new Rectangle(0x55FF0000).Expand()
                    ),
                    new Button(text: $"Hello World 🐱‍🐉 {state}", click: () => ++state.Value)
                        .PadUniform(10)
                ));
        }
    }
}
