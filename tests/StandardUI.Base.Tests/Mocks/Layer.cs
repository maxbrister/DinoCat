using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Base.Tests.Mocks
{
    class Layer : ILayer
    {
        List<Layer> children = new();
        ILayerNode? node;

        public Layer(ILayerNode? node = null)
        {
            this.node = node;
        }

        public ILayer AddChild(ILayerNode child)
        {
            Layer layer = new(child);
            children.Add(layer);
            return layer;
        }

        public void Dispose() { }

        public void Focus() { }

        public void FocusNext() { }

        public void FocusPrevious() { }

        public void InvalidateLayout() { }

        public void InvalidateRender() { }

        public void OnArrange((Size, float?) size) { }

        public void OnRender(DrawingContext drawingContext)
        {
            throw new NotImplementedException();
        }

        public void OnUpdated() { }

        public void Reparent(ILayer newParent) { }
    }
}
