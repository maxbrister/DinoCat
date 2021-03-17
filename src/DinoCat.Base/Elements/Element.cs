using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public abstract class Element
    {
        public abstract Node CreateNode(Node? parent, Context context);
    }
}
