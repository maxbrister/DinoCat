using DinoCat.Elements.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class ButtonBase : Control<State.ButtonState>
    {
        public ButtonBase()
        { }

        public override Element Build(Context context, ButtonState state)
        {
            throw new NotImplementedException();
        }
    }

    namespace State
    {
        public class ButtonState : IState
        {
            private bool mouseOver;
            private bool spaceDown;
            private bool focused;

            public event EventHandler<EventArgs>? StateChanged;

            public bool Highlight => MouseOver || SpaceDown;

            public bool MouseOver
            {
                get => mouseOver;
                set => SetValue(ref mouseOver, value);
            }

            public bool SpaceDown
            {
                get => spaceDown;
                set => SetValue(ref spaceDown, value);
            }

            public bool Focused
            {
                get => focused;
                set
                {
                    SetValue(ref focused, value);

                    if (!value)
                        SpaceDown = false;
                }
            }

            public void Dispose() { }

            private bool SetValue<T>(ref T field, T value) where T : notnull
            {
                if (!Equals(field, value))
                {
                    field = value;
                    StateChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }

                return false;
            }
        }
    }
}
