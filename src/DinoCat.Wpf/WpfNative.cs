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

using DrawingContext = DinoCat.Drawing.DrawingContext;

namespace DinoCat.Wpf
{
    public class WpfNative<TNative> : Element where TNative : UIElement, new()
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
        TNative control;
        NativeLayer layer;

        public WpfNativeNode(Node? parent, Context context, WpfNative<TNative> element) : base(parent, context, element)
        {
            control = element.Create();

            var layerNode = new NativeLayerNode(control);
            layer = (NativeLayer)context.Layer.AddChild(layerNode);
            Element.Update(control);
        }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        protected override (DinoCat.Size, float?) ArrangeOverride(DinoCat.Size availableSize)
        {
            // Wpf controls need to know when their offset changes. Currently dino controls don't.
            // Need a good way to notify controls when their offset changes. Maybe an event on NativeLayer?
            // For now invalidating render works in most cases.
            Context.InvalidateRender();
            control.Measure(availableSize.Into());
            return (control.DesiredSize.Into(), null);
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

        protected override void RenderOverride(DrawingContext context)
        {
            // TODO support rotation
            var currentTransform = context.TotalTransform;
            var parentTransform = layer.Parent.TotalTransform;
            var parentOffsetX = parentTransform.m31 / parentTransform.m11;
            var parentOffsetY = parentTransform.m32 / parentTransform.m22;
            var offsetX = currentTransform.m31 / currentTransform.m11;
            var offsetY = currentTransform.m32 / currentTransform.m22;
            offsetX -= parentOffsetX;
            offsetY -= parentOffsetY;
            control.Arrange(new global::System.Windows.Rect(offsetX, offsetY, Size.Width, Size.Height));
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

        public void RenderLayer(DrawingContext context) => throw new NotImplementedException();
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

        public void OnArrange((Size, float?) size) => throw new NotImplementedException();

        public void OnRender(DrawingContext drawingContext) => throw new NotImplementedException();

        public void OnUpdated() => throw new NotImplementedException();

        public void Reparent(ILayer newParent)
        {
            Parent.RemoveChild(Element);
            Parent = (Layer)newParent;
            Parent.AddChild(Element);
        }
    }
}
