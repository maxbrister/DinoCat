using DinoCat;
using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Interop;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DinoCat.Wpf
{
    public class WpfNative<TNative> : Element where TNative: UIElement, new()
    {
        public WpfNative(Func<TNative> create, Action<TNative> update)
        {
            Create = create;
            Update = update;
        }

        public WpfNative(Action<TNative> update)
        {
            Create = () => new();
            Update = update;
        }

        public Func<TNative> Create { get; }
        public Action<TNative> Update { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new WpfNativeNode<TNative>(parent, context, this);
    }

    internal class WpfNativeNode<TNative> : NodeBase<WpfNative<TNative>> where TNative : UIElement, new()
    {
        private TNative control;
        private NativeLayer layer;

        public WpfNativeNode(Node? parent, Context context, WpfNative<TNative> element) : base(parent, context, element)
        {
            control = element.Create();

            var layerNode = new NativeLayerNode(control);
            layer = (NativeLayer)context.Layer.AddChild(layerNode);
            Element.Update(control);
        }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        protected override DinoCat.Size ArrangeOverride(DinoCat.Size availableSize)
        {
            control.Measure(availableSize.Into());
            return control.DesiredSize.Into();
        }

        public override void Dispose()
        {
            base.Dispose();
            layer.Dispose();
            (control as IDisposable)?.Dispose();
        }

        public override IEnumerable<(Node, DinoCat.Point)> HitTest(DinoCat.Point p)
        {
            yield return (this, p);
        }

        protected override void RenderOverride(IDrawingContext context)
        {
            // TODO support rotation/scale
            var currentTransform = context.TotalTransform;
            var parentTransform = layer.Parent.TotalTransform;
            var parentOffsetX = parentTransform.m31;
            var parentOffsetY = parentTransform.m32;
            var offsetX = currentTransform.m31;
            var offsetY = currentTransform.m32;
            offsetX -= parentOffsetX;
            offsetY -= parentOffsetY;
            control.Arrange(new System.Windows.Rect(offsetX, offsetY, Size.Width, Size.Height));
            layer.Parent.OnChildRendered(control);
        }

        protected override void UpdateElement(WpfNative<TNative> oldElement, Context oldContext)
        {
            if (oldContext.Layer != Context.Layer)
            {
                layer.Dispose();

                var layerNode = new NativeLayerNode(control);
                layer = (NativeLayer)Context.Layer.AddChild(layerNode);
            }

            Element.Update(control);
        }
    }

    internal class NativeLayerNode : ILayerNode
    {
        public NativeLayerNode(UIElement native) => Native = native;

        public UIElement Native { get; }

        public void RenderLayer(IDrawingContext context) => throw new NotImplementedException();
    }

    internal class NativeLayer : ILayer
    {

        public NativeLayer(Layer parent, UIElement element)
        {
            Parent = parent;
            Element = element;
        }

        public UIElement Element { get; }
        public Layer Parent { get; private set; }

        public ILayer AddChild(ILayerNode? child) => throw new NotImplementedException();

        public void Dispose() => Parent.RemoveChild(Element);

        public void Focus() => throw new NotImplementedException();

        public void FocusNext() => throw new NotImplementedException();

        public void FocusPrevious() => throw new NotImplementedException();

        public void InvalidateLayout() => throw new NotImplementedException();

        public void InvalidateRender() => throw new NotImplementedException();

        public void OnArrange(Size size) => throw new NotImplementedException();

        public void OnRender(IDrawingContext drawingContext) => throw new NotImplementedException();

        public void OnUpdated() => throw new NotImplementedException();

        public void Reparent(ILayer newParent)
        {
            Parent.RemoveChild(Element);
            Parent = (Layer)newParent;
            Parent.AddChild(Element);
        }
    }
}
