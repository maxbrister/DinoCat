﻿using DinoCat;
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

using WpfSize = System.Windows.Size;
using WpfRect = System.Windows.Rect;
using System.Windows.Controls;

namespace DinoCat.Wpf
{
    public class Host : FrameworkElement
    {
        private StateManager stateManager;
        private Root root;
        private Layer rootLayer;
        private (WpfSize, WpfSize)? lastMeasure;
        private DpiScale? dpi;

        public Host()
        {
            var source = PresentationSource.FromVisual(this);
            var source2 = PresentationSource.CurrentSources.Cast<PresentationSource>().FirstOrDefault();
            stateManager = new StateManager(action => Dispatcher.BeginInvoke(DispatcherPriority.DataBind, action));
            rootLayer = new Layer();
            AddVisualChild(rootLayer);
            root = new Root(
                CreateContext(),
                root: () => new Dummy());
        }

        public Func<Element> RootElement
        {
            get => root.RootElement;
            set => root.RootElement = value;
        }

        protected override WpfSize ArrangeOverride(WpfSize finalSize)
        {
            // rootLayer.Arrange(new WpfRect(finalSize));

            if (lastMeasure is var (measureAvailable, measureResult))
                if (finalSize == measureAvailable)
                    return measureResult;

            var size = root.Arrange(finalSize.Into()).Into();
            return size;
        }

        protected override WpfSize MeasureOverride(WpfSize availableSize)
        {
            CheckDpi();
            rootLayer.Measure(availableSize);
            var size = root.Arrange(availableSize.Into()).Into();
            lastMeasure = (availableSize, size);
            return size;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return rootLayer;
            else
                throw new IndexOutOfRangeException();
        }

        protected override int VisualChildrenCount => 1;

        protected override void OnRender(DrawingContext drawingContext)
        {
            CheckDpi();
            var adapter = new DrawingAdapter(drawingContext);
            root.Render(adapter);
            base.OnRender(drawingContext);
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
            dpi = newDpi;
            root.Context = CreateContext();
        }

        private Context CreateContext() =>
            new Context(stateManager, rootLayer, new Dictionary<Type, object>
            {
                { typeof(ITypeface),
                    new DinoTypeface(
                        new Typeface(
                            new System.Windows.Controls.TextBlock().FontFamily,
                            FontStyles.Normal, System.Windows.FontWeights.Normal, FontStretches.Normal)) },
                // TODO can we come up with a better default?
                {  typeof(IFontManager), new FontManager(dpi?.DpiScaleX ?? 2) }
            });
        
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