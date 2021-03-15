using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Interop;
using DinoCat.Wpf.Automation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;
using Colors = System.Windows.Media.Colors;
using WpfRect = System.Windows.Rect;
using WpfSize = System.Windows.Size;

namespace DinoCat.Wpf
{
    internal class Layer : FrameworkElement, ILayer
    {
        protected ILayerNode? root;
        private Layer? parent;
        private List<UIElement> children = new List<UIElement>();
        private List<UIElement> renderedChildren = new List<UIElement>();
        private double nextDesiredSize = 0;
        private WpfSize size;

        public Layer() { }

        public Layer(Layer parent, ILayerNode root)
        {
            this.root = root;
            this.parent = parent;
            IsHitTestVisible = false;
        }

        public DinoCat.Matrix TotalTransform { get; private set; }

        public void AddChild(UIElement child)
        {
            children.Add(child);
            AddVisualChild(child);
        }

        ILayer ILayer.AddChild(ILayerNode child)
        {
            Layer layer;
            if (child is InputNode input)
                layer = new InputLayer(this, input);
            else if (child is NativeLayerNode native)
            {
                var nativeLayer = new NativeLayer(this, native.Native);
                AddChild(native.Native);
                return nativeLayer;
            }
            else
                layer = new Layer(this, child);
            AddChild(layer);
            return layer;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            parent?.RemoveChild(this);
        }

        public void RemoveChild(UIElement child)
        {
            children.Remove(child);
            RemoveVisualChild(child);
        }

        public void OnChildRendered(UIElement child) =>
            renderedChildren.Add(child);

        protected override Visual GetVisualChild(int index) =>
            children[index];

        protected override int VisualChildrenCount => children.Count;

        protected override WpfSize MeasureOverride(WpfSize availableSize)
        {
            nextDesiredSize += 1;
            return new WpfSize(nextDesiredSize, nextDesiredSize);
        }

        protected override WpfSize ArrangeOverride(WpfSize finalSize)
        {
            // TODO might need to fix Stack
            if (root != null && size.Width > 0 && size.Height > 0)
                return size;
            else
                return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            root?.RenderLayer(new DrawingAdapter(drawingContext));
            if (renderedChildren.Count > 0)
            {
                children = renderedChildren;
                renderedChildren = new List<UIElement>();
            }
            base.OnRender(drawingContext);
        }

        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (root != null)
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            else
                return null;
        }

        public void OnRemove(object native)
        {
            if (native is UIElement element)
                RemoveChild(element);
        }

        public void OnArrange(Size size) => this.size = size.Into();

        void ILayer.OnRender(IDrawingContext drawingContext)
        {
            parent?.OnChildRendered(this);

            TotalTransform = drawingContext.TotalTransform;

            // TODO support rotation/scale
            var parentTransform = parent?.TotalTransform ?? DinoCat.Matrix.Identity;
            var parentOffsetX = parentTransform.m31;
            var parentOffsetY = parentTransform.m32;
            var offsetX = TotalTransform.m31;
            var offsetY = TotalTransform.m32;
            offsetX -= parentOffsetX;
            offsetY -= parentOffsetY;
            Arrange(new System.Windows.Rect(offsetX, offsetY, size.Width, size.Height));
        }

        public virtual void OnUpdated() { }

        public void InvalidateRender()
        {
            InvalidateVisual();
            parent?.InvalidateRender();
            if (VisualParent is Host host)
                host.InvalidateVisual();
        }

        public void InvalidateLayout()
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        void IFocus.Focus() => Focus();
        void IFocus.FocusNext() { }
        void IFocus.FocusPrevious() { }

        public void Reparent(ILayer newParent)
        {
            parent?.RemoveChild(this);
            ((Layer)newParent).AddChild(this);
        }
    }
}
