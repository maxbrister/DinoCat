using DinoCat.Base.Elements;
using DinoCat.Base.Interop;
using System;
using System.Windows;
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
                scheduleUpdate: action => Dispatcher.BeginInvoke(DispatcherPriority.DataBind, action),
                root: () => new Dummy());
        }

        public Func<Element> RootElement
        {
            get => root.RootElement;
            set => root.RootElement = value;
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
