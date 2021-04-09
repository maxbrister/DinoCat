using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Interop
{
    public interface IFocus
    {
        void Focus();
        void FocusNext();
        void FocusPrevious();
    }
}
