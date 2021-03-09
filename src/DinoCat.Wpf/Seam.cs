using DinoCat.Base;
using DinoCat.Base.Controls;
using DinoCat.Base.Interop;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DinoCat.Wpf
{
    public class Seam : FrameworkElement
    {
        private Root root;

        public Seam()
        {
            root = new Root(
                scheduleUpdateState: action => Dispatcher.BeginInvoke(DispatcherPriority.DataBind, action),
                genRoot: () => new Dummy());
        }

        public Func<Element> GenRoot
        {
            get => root.GenRoot;
            set => root.GenRoot = value;
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            var size = root.Arrange(finalSize.Into()).Into();
            return size;
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            var size = root.Arrange(availableSize.Into()).Into();
            return size;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var adapter = new DrawingAdapter(drawingContext);
            root.Render(adapter);
        }
    }
}
