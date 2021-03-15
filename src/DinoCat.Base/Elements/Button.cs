using DinoCat.Drawing;
using DinoCat.Elements.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Button : Control<State.ButtonState>
    {
        public Button(Action click, string text) : this(click, new Text(text))
        { }

        public Button(Action click, Element content)
        {
            Click = click;
            Content = content;
        }

        public Action Click { get; }
        public Element Content { get; }

        public override Element Build(Context context, State.ButtonState state) =>
            new Input(
                controlType: ControlType.Button,
                tap: () => Click(),
                mouseEnter: () => state.MouseOver = true,
                mouseExit: () => state.MouseOver = false,
                keyDown: e =>
                {
                    switch (e.Key)
                    {
                        case Key.Space:
                            state.SpaceDown = true;
                            break;
                        case Key.Enter:
                            Click();
                            break;
                    }
                },
                keyUp: e =>
                {
                    if (e.Key == Key.Space && state.SpaceDown)
                    {
                        state.SpaceDown = false;
                        Click();
                        e.Handled = true;
                    }
                },
                gotFocus: fromKeyboard => state.Focused = fromKeyboard,
                lostFocus: () => state.Focused = false,
                child: new Stack(
                        new Rectangle(
                            new Paint(
                                fill: state.Highlight ? 0xFFBEE6FD : 0xFFDDDDDD,
                                stroke: state.Highlight ? 0xFF3C7FB1 : 0xFF707070)
                            ).Expand(),
                        new Stack(
                            Content,
                            new Rectangle(
                                new Paint(
                                    stroke: state.Focused ? new Pen(Colors.Black,
                                        width: .75f,
                                        dashStyle: new DashStyle(new double[] { .25f, 4 }, 0)) : null)
                                ).Expand()
                        ).PadUniform(2)
                    )
                );
    }
}
