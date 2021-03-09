using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Controls
{
    public class Button : Control
    {
        public override Element Build(BuildContext context)
        {
            return new Button();
        }
    }
}
