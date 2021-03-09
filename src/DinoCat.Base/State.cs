using System;
using System.Threading;

namespace DinoCat.Base
{
    public class State : IState
    {
        private int disposed;
        public event EventHandler<EventArgs>? StateChanged;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
                Disposing();
        }

        protected virtual void Disposing()
        { }

        protected void Notify() =>
            StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
