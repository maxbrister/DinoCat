using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Drawing
{
    public interface IFormattedText
    {
        string Text { get; }
        float Width { get; }
        float Height { get; }
        object NativeObject { get; }
    }
}
