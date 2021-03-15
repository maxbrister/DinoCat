using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public interface IFormattedText
    {
        public string Text { get; }
        public double Width { get; }
        public double Height { get; }
        public object NativeObject { get; }
    }
}
