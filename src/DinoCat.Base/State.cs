using System;

namespace DinoCat.Base
{
    public class State : IState
    {
        public event EventHandler<EventArgs>? StateChanged;

        protected void Notify() =>
            StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
