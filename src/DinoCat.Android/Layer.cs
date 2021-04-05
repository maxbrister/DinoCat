using Android.Runtime;
using Android.Util;
using Android.Views;
using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Interop;
using DinoCat.Tree;
using SkiaSharp.Views.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Diagnostics.Debug;

namespace DinoCat.Android
{
    class Layer : ViewGroup, ILayer
    {
        Action<DrawingContext> render;
        SKGLTextureView visual;
        List<View> renderedChildren = new();
        List<View> drawOrder = new();
        float scaledDensity;
        int arrangeWidth;
        int arrangeHeight;

        public Layer(global::Android.Content.Context? context, Action<DrawingContext> render) : base(context)
        {
            this.render = render;
            visual = new(context);
            AddView(visual);
            visual.PaintSurface += Visual_PaintSurface;

            DisplayMetrics metrics = new();
            Context?.Display?.GetRealMetrics(metrics);
            scaledDensity = metrics.ScaledDensity;
        }

        void Visual_PaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
        {
            // TODO adjust dpi/ect, only redraw if we're dirty
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear();

            canvas.Save();
            canvas.Scale(scaledDensity);
            try
            {
                DrawingContext drawingContext = new(canvas);
                render(drawingContext);
            }
            finally
            {
                canvas.Restore();
            }

            drawOrder = renderedChildren;
            renderedChildren = new();
        }

        ILayer ILayer.AddChild(ILayerNode child)
        {
            Layer layer;
            if (child is InputNode input)
                layer = new InputLayer(Context, input);
            else
                layer = new Layer(Context, ctx => child.RenderLayer(ctx));
            return layer;
        }

        public virtual void OnUpdated() { }

        void IFocus.Focus() => RequestFocus();

        void IFocus.FocusNext() => RequestFocus(FocusSearchDirection.Forward);

        void IFocus.FocusPrevious() => RequestFocus(FocusSearchDirection.Backward);

        void ILayer.InvalidateLayout() => RequestLayout();

        void ILayer.InvalidateRender() => Invalidate();

        void ILayer.OnArrange(Size size)
        {
            WriteLine($"Layer.OnArrange({size})");
            arrangeWidth = (int)Math.Ceiling(size.Width * scaledDensity);
            arrangeHeight = (int)Math.Ceiling(size.Height * scaledDensity);
        }

        void ILayer.OnRender(DrawingContext drawingContext)
        {
            var parent = Parent as Layer;
            parent?.OnChildRendered(this);

            TotalTransform = drawingContext.TotalTransform;

            // TODO support rotation/scale/ect?
            var parentTransform = parent?.TotalTransform ?? DinoCat.Matrix.Identity;
            var parentOffsetX = parentTransform.m31 / parentTransform.m11;
            var parentOffsetY = parentTransform.m32 / parentTransform.m22;
            var offsetX = TotalTransform.m31 / TotalTransform.m11;
            var offsetY = TotalTransform.m32 / TotalTransform.m22;
            offsetX -= parentOffsetX;
            offsetY -= parentOffsetY;

            // API is intended for animations. Idea is we want to delay layout until right before we render.
            // If this doesn't pan out we could run 🐱‍🐉 render during layout and store the actual render commands/reply during
            // render.
            SetLeftTopRightBottom((int)offsetX, (int)offsetY, (int)(offsetX + arrangeWidth), (int)(offsetY + arrangeHeight));
            visual.SetLeftTopRightBottom(0, 0, arrangeWidth, arrangeHeight);
        }

        public override void Draw(global::Android.Graphics.Canvas? canvas)
        {
            base.Draw(canvas);
        }

        protected override int GetChildDrawingOrder(int childCount, int drawingPosition)
        {
            if (drawOrder.Count  == childCount + 1)
            {
                if (drawingPosition == 0)
                    return 0;

                // TODO O(n^2) is bad here
                var drawMe = drawOrder[drawingPosition];
                for (int i = 1; i < childCount; ++i)
                {
                    if (GetChildAt(i) == drawMe)
                        return i;
                }
            }

            return base.GetChildDrawingOrder(childCount, drawingPosition);
        }

        void ILayer.Reparent(ILayer newParent)
        {
            ((ViewGroup?)Parent)?.RemoveView(this);
            ((ViewGroup)newParent).AddView(this);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            WriteLine($"Layer.OnMeasure: {arrangeWidth}, {arrangeHeight}");
            // TODO deal with rotation/scale in 🐱‍🐉
            MeasureChildren(
                MeasureSpec.MakeMeasureSpec(arrangeWidth, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec(arrangeHeight, MeasureSpecMode.Exactly));
            SetMeasuredDimension(arrangeWidth, arrangeHeight);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var childCount = ChildCount;
            for (int i = 0; i < childCount; ++i)
                GetChildAt(i)?.Layout(0, 0, arrangeWidth, arrangeHeight);
        }

        Matrix TotalTransform { get; set; } = DinoCat.Matrix.Identity;

        void OnChildRendered(Layer child) => renderedChildren.Add(child);
    }
}
