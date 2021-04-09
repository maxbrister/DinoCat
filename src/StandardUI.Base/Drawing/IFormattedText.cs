using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Drawing
{
    public interface IFormattedText
    {
        string Text { get; }
        float Width { get; }
        float Height { get; }
        float Ascent { get; }
        float Descent { get; }
        object NativeObject { get; }
    }
}
