using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements.Events
{
    public class KeyEvent
    {
        public KeyEvent(Key key, bool isRepeat, KeyState state)
        {
            IsRepeat = isRepeat;
            Key = key;
            State = state;
        }

        public bool IsRepeat { get; }
        public Key Key { get; }
        public KeyState State { get; }
        public bool Handled { get; set; }
    }
}
