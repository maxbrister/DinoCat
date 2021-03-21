using DinoCat;
using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Interop;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using DrawingContext = DinoCat.Drawing.DrawingContext;
using WpfDrawingContext = System.Windows.Media.DrawingContext;
using WpfSize = System.Windows.Size;
using WpfRect = System.Windows.Rect;
using SkiaSharp;

namespace DinoCat.Wpf
{
    public class Host : System.Windows.Controls.Control
    {
        private StateManager stateManager;
        protected Root root;
        private RootLayer rootLayer;
        private DpiScale? dpi;
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

        private void Root_RootNodeChanged(object? sender, EventArgs e) =>
            rootLayer.RootNode = root.RootNode;

        public Func<Element> RootElement
        {
            get => root.RootElement;
            set => root.RootElement = value;
        }

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

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
            dpi = newDpi;
            root.Context = CreateContext();
        }

        private Context CreateContext()
        {
            return new Context(stateManager, rootLayer, new Dictionary<Type, object>
            {
                { typeof(ITypeface), fontManager.CreateTypeface("Segoe UI Emoji") },
                // TODO can we come up with a better default?
                {  typeof(IFontManager), fontManager }
            });
        }

        private void CheckDpi()
        {
            if (dpi == null)
            {
                var source = PresentationSource.FromVisual(this);
                if (source != null)
                {
                    var dpix = source.CompositionTarget.TransformToDevice.M11;
                    var dpiy = source.CompositionTarget.TransformToDevice.M22;
                    dpi = new DpiScale(dpix, dpiy);
                    if (dpix != 2)
                        root.Context = CreateContext();
                }
            }
        }
    }
}
