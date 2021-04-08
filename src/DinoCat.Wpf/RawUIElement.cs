using DinoCat.Elements;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DinoCat.Wpf
{
    public class RawUIElement : Element
    {
        public RawUIElement(UIElement element) => Element = element;

        public UIElement Element { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new RawUIElementNode(parent, context, this);
    }

    internal class RawUIElementNode : NodeBase<RawUIElement>
    {
        NativeLayer layer;

        public RawUIElementNode(Node? parent, Context context, RawUIElement element) : base(parent, context, element)
        {
            var layerNode = new NativeLayerNode(Current);
            layer = (NativeLayer)context.Layer.AddChild(layerNode);
        }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        protected override (Size, float?) ArrangeOverride(Size availableSize)
        {
            Current.Measure(availableSize.Into());
            return (Current.DesiredSize.Into(), null);
        }

        public override void Dispose()
        {
            base.Dispose();
            layer.Dispose();
        }

        protected override void RenderOverride(Drawing.DrawingContext context)
        {
            // At some point RawUIElement/WpfNative should be combined
            // TODO support rotation/scale
            var currentTransform = context.TotalTransform;
            var parentTransform = layer.Parent.TotalTransform;
            var parentOffsetX = parentTransform.m31;
            var parentOffsetY = parentTransform.m32;
            var offsetX = currentTransform.m31;
            var offsetY = currentTransform.m32;
            offsetX -= parentOffsetX;
            offsetY -= parentOffsetY;
            Current.Arrange(new global::System.Windows.Rect(offsetX, offsetY, Size.Width, Size.Height));
            layer.Parent.OnChildRendered(Current);
        }

        protected override void UpdateElement(RawUIElement oldElement, Context oldContext)
        {
            if (oldContext.Layer != Context.Layer ||
                oldElement.Element != Current)
            {
                layer.Dispose();

                var layerNode = new NativeLayerNode(Current);
                layer = (NativeLayer)Context.Layer.AddChild(layerNode);
            }
        }

        private UIElement Current => Element.Element;
    }
}
