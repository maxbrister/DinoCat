using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Interop
{
    public interface ILayer : IFocus, IDisposable
    {
        public ILayer AddChild(ILayerNode child);
        public void OnArrange((Size, float?) size);
        public void OnRender(DrawingContext drawingContext);
        public void OnUpdated();
        public void InvalidateLayout();
        public void InvalidateRender();
        public void Reparent(ILayer newParent);
    }
}
