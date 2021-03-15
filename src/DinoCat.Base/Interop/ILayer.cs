using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Interop
{
    public interface ILayer : IFocus, IDisposable
    {
        public ILayer AddChild(ILayerNode child);
        public void OnArrange(Size size);
        public void OnRender(IDrawingContext drawingContext);
        public void OnUpdated();
        public void InvalidateLayout();
        public void InvalidateRender();
        public void Reparent(ILayer newParent);
    }
}
