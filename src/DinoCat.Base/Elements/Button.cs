using DinoCat.Drawing;
using DinoCat.Elements.Events;
using DinoCat.Elements.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Button : Control<ButtonState>
    {
        public Button(Action click, string text) : this(click, new Text(text))
        { }

        public Button(Action click, Element content)
        {
            Click = click;
            Content = content;
        }

        public Button(Action click, Func<Element> content)
        {
            Click = click;
            Content = new ScopeElement(content);
        }

        public Action Click { get; }
        public Element Content { get; }

        public override Element Build(Context context, ButtonState state, Action<ButtonState> setState) =>
            new Input(
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
                gotFocus: fromKeyboard => setState(state with { Focused = fromKeyboard }),
                lostFocus: () => setState(state with { Focused = false, SpaceDown = false }),
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
                                        dashStyle: new DashStyle(new[] { .25f, 4 }, 0)) : null)
                                ).Expand()
                        ).Margin(2)
                    )
                );
    }

    namespace State
    {
        public record ButtonState(bool MouseOver = false, bool SpaceDown = false, bool Focused = false)
        {
            public ButtonState() : this(false, false, false) { }
            public bool Highlight => MouseOver || SpaceDown;
        }
    }
}
