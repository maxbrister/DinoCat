using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Tree;
using System;

namespace DinoCat.Interop
{
    public class Root
    {
        private Func<Element> root;
        private Node rootNode;
        private Context context;

        public event EventHandler<EventArgs>? RootNodeChanged;

        public Root(Context context, Func<Element> root)
        {
            this.context = context;
            this.root = root;

            var rootElement = root();
            rootNode = rootElement.CreateNode(null, context);
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

        public Node RootNode
        {
            get => rootNode;
            private set
            {
                if (rootNode != value)
                {
                    rootNode.Dispose();
                    rootNode = value;
                    RootNodeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Context Context
        {
            get => context;
            set
            {
                if (context != value)
                {
                    context = value;

                    var currentRoot = root();
                    RootNode = rootNode.UpdateElement(currentRoot, context);
                }
            }
        }

        public Size Arrange(Size availableSpace) =>
            rootNode.Arrange(availableSpace).Item1;

        public void Render(DrawingContext context) =>
            rootNode.Render(context);

        public void Refresh()
        {
            // TODO: Hook into hot reload infrastructure here
            var newRoot = root();
            RootNode = rootNode.UpdateElement(newRoot, rootNode.Context);
        }
    }
}
