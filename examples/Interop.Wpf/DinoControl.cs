using DinoCat;
using DinoCat.Elements;
using DinoCat.Wpf;
using System;

using WpfButton = DinoCat.Wpf.System.Windows.Controls.Button;

namespace Interop.Wpf
{
    [ToWpfType("Interop.Wpf.DinoControlWrapper")]
    class DinoControl : DinoCat.Elements.Control<int>
    {
        public override Element Build(Context context, int state, Action<int> setState) =>
            new Column(
                new MyUserControlWrapper().MyValue(state),
                new WpfButton()
                    .Content("Increment 🐱‍🐉")
                    .On(WpfButton.ClickEvent, args => setState(state + 1))
                    .Margin(2).Center(),
                new MyUserControlWrapper().MyValue(-state));
    }
}
