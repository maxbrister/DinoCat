using DinoCat.Drawing;
using DinoCat.Elements.Events;
using DinoCat.Elements.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DinoCat.Elements.Factories;

namespace DinoCat.Elements
{
    public class Button : Control<ButtonState>
    {
        public Button(Element content, Action click)
        {
            Click = click;
            Content = content;
        }

        public Action Click { get; }
        public Element Content { get; }

        public override Element Build(Context context, ButtonState state, Action<ButtonState> setState) =>
            Input(
                controlType: ControlType.Button,
                tap: () => Click(),
                mouseEnter: () => setState(state with { MouseOver = true }),
                mouseExit: () => setState(state with { MouseOver = false }),
                keyDown: e =>
                {
                    switch (e.Key)
                    {
                        case Key.Space:
                            e.Handled = true;
                            setState(state with { SpaceDown = true });
                            break;
                        case Key.Enter:
                            e.Handled = true;
                            Click();
                            break;
                    }
                },
                keyUp: e =>
                {
                    if (e.Key == Key.Space)
                    {
                        setState(state with { SpaceDown = false });
                        Click();
                        e.Handled = true;
                    }
                },
                gotFocus: visualizeFocus => setState(state with { VisualizeFocus = visualizeFocus }),
                lostFocus: () => setState(state with { VisualizeFocus = false, SpaceDown = false }),
                child: Stack(
                        Rectangle(
                            new Paint(
                                fill: state.Highlight ? 0xFFBEE6FD : 0xFFDDDDDD,
                                stroke: state.Highlight ? 0xFF3C7FB1 : 0xFF707070)
                            ).Expand(),
                        Stack(
                            Content,
                            Rectangle(
                                new Paint(
                                    stroke: state.VisualizeFocus ? new Pen(Colors.Black,
                                        width: 1,
                                        dashStyle: new DashStyle(new float[] { 2, 2.5f }, 0)) : null)
                                ).Expand()
                        ).Margin(2)
                    )
                );
    }

    namespace State
    {
        public record ButtonState(bool MouseOver = false, bool SpaceDown = false, bool VisualizeFocus = false)
        {
            public ButtonState() : this(false, false, false) { }
            public bool Highlight => MouseOver || SpaceDown;
        }
    }
}
