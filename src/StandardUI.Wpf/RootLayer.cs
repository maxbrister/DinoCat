using Microsoft.StandardUI.Tree;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using DrawingContext = Microsoft.StandardUI.Drawing.DrawingContext;
using WpfSize = System.Windows.Size;

namespace Microsoft.StandardUI.Wpf
{
    internal class RootLayer : Layer
    {
        private Node? rootNode;
        private (WpfSize, WpfSize)? layout;

        public RootLayer() { }

        public Node? RootNode
        {
            get => rootNode;
            set
            {
                if (rootNode != value)
                {
                    rootNode = value;
                    InvalidateLayout();
                    InvalidateRender();
                }
            }
        }

        protected override WpfSize MeasureOverride(WpfSize availableSize) => DoLayout(availableSize);

        protected override WpfSize ArrangeOverride(WpfSize finalSize)
        {
            if (layout is (WpfSize originalAvailable, WpfSize result) &&
                (originalAvailable == finalSize || finalSize == result))
                return result;
            return DoLayout(finalSize);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Paint(e, context => rootNode?.Render(context));
            base.OnPaintSurface(e);
        }

        private WpfSize DoLayout(WpfSize availableSize)
        {
            var (desired, _) = rootNode!.Arrange(availableSize.Into());
            layout = new(availableSize, desired.Into());
            return layout.Value.Item2;
        }
    }
}
