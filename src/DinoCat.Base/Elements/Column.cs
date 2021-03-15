using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Column : Flex
    {
        public Column(params Expand[] children) : base(Orientation.Vertical, children)
        {
        }
    }
}
