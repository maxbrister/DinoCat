using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Interop;
using DinoCat.Tree;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Diagnostics.Debug;

namespace DinoCat.Android
{
    public class Host : ViewGroup
    {
        static Handler toMainThread = new(Looper.MainLooper!);
        static FontManager fontManager = new(SKFontManager.Default);
        StateManager stateManager = new(action => toMainThread.Post(action));
        Root root;
        Layer rootLayer;

        public Host(global::Android.Content.Context? context) : base(context)
        {
            rootLayer = new(context, context => root!.Render(context));
            root = new Root(CreateContext(context), () => new Dummy());
            Initialize();
        }

        public Host(global::Android.Content.Context? context, IAttributeSet? attrs) : base(context, attrs)
        {
            // TODO extract font from attrs
            rootLayer = new(context, context => root!.Render(context));
            root = new Root(CreateContext(context), () => new Dummy());
            Initialize();
        }

        public Host(global::Android.Content.Context? context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            rootLayer = new(context, context => root!.Render(context));
            root = new Root(CreateContext(context), () => new Dummy());
            Initialize();
        }

        public Host(global::Android.Content.Context? context, IAttributeSet? attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            rootLayer = new(context, context => root!.Render(context));
            root = new Root(CreateContext(context), () => new Dummy());
            Initialize();
        }

        public Func<Element> RootElement
        {
            get => root.RootElement;
            set => root.RootElement = value;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // TODO: Deal with whole measure spec
            var dpiScale = root.Context.Get<DpiScale>();
            int availableWidth = MeasureSpec.GetSize(widthMeasureSpec);
            int availableHeight = MeasureSpec.GetSize(heightMeasureSpec);
            var size = root.Arrange(new(availableWidth, availableHeight));
            ((ILayer)rootLayer).OnArrange(size);

            int measuredWith = (int)Math.Ceiling(size.Width * dpiScale.X);
            int measuredHeight = (int)Math.Ceiling(size.Height * dpiScale.Y);
            rootLayer.Measure(
                MeasureSpec.MakeMeasureSpec(measuredWith, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec(measuredHeight, MeasureSpecMode.Exactly));
            SetMeasuredDimension(measuredWith, measuredHeight);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom) =>
            rootLayer.Layout(0, 0, right - left, bottom - top);

        // It would be nice if we could initialize based on the android context here, but
        // C# nullable reference support doesn't really support that.
        void Initialize() => AddView(rootLayer);

        Context CreateContext(global::Android.Content.Context? context)
        {
            DisplayMetrics metrics = new();
            context?.Display?.GetRealMetrics(metrics);
            DpiScale scale = new(metrics.ScaledDensity, metrics.ScaledDensity);
            return new Context(stateManager, rootLayer, new Dictionary<Type, object>
            {
                { typeof(IFontManager), fontManager },
                { typeof(TextTheme), new TextTheme(fontManager.CreateTypeface("sans-serif"), 16, Colors.Black) },
                { typeof(DpiScale), scale }
            });
        }
    }
}
