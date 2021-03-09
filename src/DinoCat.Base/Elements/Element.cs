using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Elements
{
    public interface Element
    {
        Node CreateNode(int depth, BuildContext context);
    }
}
