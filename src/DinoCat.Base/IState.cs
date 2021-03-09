using System;

namespace DinoCat.Base
{
    public interface IState
    {
        event EventHandler<EventArgs> StateChanged;
    }
}
