using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public interface ITypeface
    {
        public string FamilyName { get; }
        public FontSlant Slant { get; }
        public int Weight { get; }
        public object NativeObject { get; }
    }
}
