using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Android
{
    class InputLayer : Layer
    {
        public InputLayer(global::Android.Content.Context? context, InputNode input) : base(context, input.Render)
        {
            // TODO handle input
        }
    }
}
