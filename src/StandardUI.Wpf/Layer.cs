using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Elements;
using Microsoft.StandardUI.Interop;
using Microsoft.StandardUI.Wpf.Automation;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;
using Colors = System.Windows.Media.Colors;
using WpfRect = System.Windows.Rect;
using WpfSize = System.Windows.Size;
using DrawingContext = Microsoft.StandardUI.Drawing.DrawingContext;
using WpfDrawingContext = System.Windows.Media.DrawingContext;
using SkiaSharp;

namespace Microsoft.StandardUI.Wpf
{
    internal class Layer : SKElement, ILayer
    {
        protected ILayerNode? root;
        Layer? parent;
        List<UIElement> children = new List<UIElement>();
        List<UIElement> renderedChildren = new List<UIElement>();
        double nextDesiredSize = 0;
        WpfSize size;

        public Layer() { }

        public Layer(Layer parent, ILayerNode root)
        {
            this.root = root;
            this.parent = parent;
            IsHitTestVisible = false;
            Focusable = false;
        }

        public Microsoft.StandardUI.Matrix TotalTransform { get; private set; } = Microsoft.StandardUI.Matrix.Identity;

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
            renderedChildren.Remove(child);
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

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            if (root != null)
            {
                Paint(e, context => root.RenderLayer(context));
            }
        }

        protected void Paint(SKPaintSurfaceEventArgs e, Action<DrawingContext> paintImpl)
        {
            var source = PresentationSource.FromVisual(this);
            var dpix = (float)source.CompositionTarget.TransformToDevice.M11;
            var dpiy = (float)source.CompositionTarget.TransformToDevice.M22;
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            DrawingContext context = new(e.Surface.Canvas);
            try
            {
                canvas.Scale(dpix, dpiy);
                paintImpl(context);
            }
            finally
            {
                canvas.Restore();
            }
        }

        protected override void OnRender(WpfDrawingContext drawingContext)
        {
            if (renderedChildren.Count > 0)
            {
                // In some cases children will be removed before measure is called again.
                // Don't update child order unless we're up to date.
                if (children.Count == renderedChildren.Count)
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

        public void OnArrange((Size, float?) result)
        {
            this.size = result.Item1.Into();
            InvalidateVisual();
        }

        void ILayer.OnRender(DrawingContext drawingContext)
        {
            parent?.OnChildRendered(this);

            TotalTransform = drawingContext.TotalTransform;

            // TODO support rotation. Render transform somehow?
            var parentTransform = parent?.TotalTransform ?? Microsoft.StandardUI.Matrix.Identity;
            var parentOffsetX = parentTransform.m31 / parentTransform.m11;
            var parentOffsetY = parentTransform.m32 / parentTransform.m22;
            var offsetX = TotalTransform.m31 / TotalTransform.m11;
            var offsetY = TotalTransform.m32 / TotalTransform.m22;
            offsetX -= parentOffsetX;
            offsetY -= parentOffsetY;
            Arrange(new global::System.Windows.Rect(offsetX, offsetY, size.Width, size.Height));
        }

        public virtual void OnUpdated() { }

        public void InvalidateRender()
        {
            InvalidateVisual();
            parent?.InvalidateRender();
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
