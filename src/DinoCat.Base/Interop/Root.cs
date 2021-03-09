using DinoCat.Base.Controls;
using DinoCat.Base.Drawing;
using DinoCat.Base.Elements;
using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DinoCat.Base.Interop
{
    public class Root
    {
        private Func<Element> root;
        private Site rootNode;
        private BuildContext context;

        public event EventHandler<EventArgs>? ArrangeInvalidated;
        public event EventHandler<EventArgs>? RenderInvalidated;

        public Root(Action<Action> scheduleUpdate, Func<Element> root)
        {
            this.root = root;
            context = new BuildContext(
                new StateManager(scheduleUpdate),
                this.OnArrangeInvalidated,
                this.OnRenderInvalidated);

            var rootElement = root();
            rootNode = new Site(-1, context, rootElement);
        }

        public Func<Element> RootElement
        {
            get => root;
            set
            {
                root = value;
                Refresh();
            }
        }

        public Size Arrange(Size availableSpace) =>
            rootNode.Arrange(availableSpace);

        public void Render(IDrawingContext context) =>
            rootNode.Render(context);

        public void Refresh()
        {
            // TODO: Hook into hot reload infrastructure here
            var newRoot = root();
            rootNode.UpdateElement(newRoot);
        }

        private void OnArrangeInvalidated() =>
            ArrangeInvalidated?.Invoke(this, EventArgs.Empty);

        private void OnRenderInvalidated() =>
            RenderInvalidated?.Invoke(this, EventArgs.Empty);
    }
}
