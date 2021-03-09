using System;

namespace DinoCat.Base
{
    public interface IState : IDisposable
    {
        event EventHandler<EventArgs> StateChanged;
    }
}
