using DinoCat;
using DinoCat.Elements;
using DinoCat.Wpf;
using System;
using static DinoCat.Elements.Factories;
using static DinoCat.Wpf.System.Windows.Controls.Factories;
using static Interop.Wpf.Factories;

namespace Interop.Wpf
{
    [ToWpfType("Interop.Wpf.DinoControlWrapper")]
    class DinoControl : DinoCat.Elements.Control<int>
    {
        public DinoControl(string incomming) => Incomming = incomming;

        public string Incomming { get; }

        public override Element Build(Context context, int state, Action<int> setState) =>
            Column(
                TextBlock().Text(Incomming),
                MyUserControlWrapper().MyValue(state),
                Button()
                    .Content("Increment 🐱‍🐉")
                    .OnClick(args => setState(state + 1))
                    .Margin(2)
                    .Center(),
                Row(VerticalAlignment.Baseline,
                    Text("HELLO", fontSize: 32),
                    Button("world", () => { }),
                    Text(" trailing text")),
                MyUserControlWrapper().MyValue(-state));
    }
}
