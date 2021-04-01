using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Interop;
using DinoCat.Tree;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Colors = DinoCat.Drawing.Colors;
using DpiScale = DinoCat.Drawing.DpiScale;
using WpfDpiScale = System.Windows.DpiScale;
using WpfDrawingContext = System.Windows.Media.DrawingContext;
using WpfSize = System.Windows.Size;

namespace DinoCat.Wpf
{
    public class Host : global::System.Windows.Controls.Control
    {
        private StateManager stateManager;
        private Root root;
        private RootLayer rootLayer;
        // TODO default to current system dpi?
        private DpiScale dpi = new();
        private FontManager fontManager = new FontManager(SKFontManager.Default);

        public Host()
        {
            var source = PresentationSource.FromVisual(this);
            var source2 = PresentationSource.CurrentSources.Cast<PresentationSource>().FirstOrDefault();
            stateManager = new StateManager(action => Dispatcher.BeginInvoke(DispatcherPriority.DataBind, action));
            rootLayer = new RootLayer();
            AddVisualChild(rootLayer);
            root = new Root(
                CreateContext(),
                root: () => new Dummy());
            root.RootNodeChanged += Root_RootNodeChanged;
            rootLayer.RootNode = root.RootNode;
            Focusable = false;
        }

        public Host(Func<Element> rootElement): this() => RootElement = rootElement;

        private void Root_RootNodeChanged(object? sender, EventArgs e) =>
            rootLayer.RootNode = root.RootNode;

        public Func<Element> RootElement
        {
            get => root.RootElement;
            set => root.RootElement = value;
        }

        public void Refresh() => root.Refresh();

        protected override WpfSize ArrangeOverride(WpfSize finalSize)
        {
            rootLayer.Arrange(new(rootLayer.DesiredSize));
            return rootLayer.DesiredSize;
        }

        protected override WpfSize MeasureOverride(WpfSize availableSize)
        {
            CheckDpi();
            rootLayer.Measure(availableSize);
            return rootLayer.DesiredSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return rootLayer;
            else
                throw new IndexOutOfRangeException();
        }

        protected override int VisualChildrenCount => 1;

        protected override void OnRender(WpfDrawingContext drawingContext)
        {
            CheckDpi();
            base.OnRender(drawingContext);
        }

        protected override void OnDpiChanged(WpfDpiScale oldDpi, WpfDpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
            dpi = newDpi.Into();
            root.Context = CreateContext();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FontSizeProperty)
                root.Context = CreateContext();
        }

        private Context CreateContext()
        {
            var typeface = fontManager.CreateTypeface("Segoe UI Emoji");
            return new Context(stateManager, rootLayer, new Dictionary<Type, object>
            {
                { typeof(DpiScale), dpi },
                { typeof(TextTheme), new TextTheme(typeface, (float)FontSize, Colors.Black) },
                { typeof(IFontManager), fontManager }
            });
        }

        private void CheckDpi()
        {
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                var dpix = source.CompositionTarget.TransformToDevice.M11;
                var dpiy = source.CompositionTarget.TransformToDevice.M22;
                var currentDpi = new DpiScale((float)dpix, (float)dpiy);
                if (currentDpi != dpi)
                {
                    dpi = currentDpi;
                    root.Context = CreateContext();
                }
            }
        }
    }
}
