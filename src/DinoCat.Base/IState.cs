using System;

namespace DinoCat
{
    public interface IState : IDisposable
    {
        event EventHandler<EventArgs> StateChanged;
    }
}
