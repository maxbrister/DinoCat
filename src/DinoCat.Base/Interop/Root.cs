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

        public Root(Context context, Func<Element> root)
        {
            this.context = context;
            this.root = root;

            var rootElement = root();
            rootNode = rootElement.CreateNode(-1, context);
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

        public Context Context
        {
            get => context;
            set
            {
                if (context != value)
                {
                    context = value;

                    var currentRoot = root();
                    rootNode = rootNode.UpdateElement(currentRoot, context);
                }
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
            rootNode = rootNode.UpdateElement(newRoot);
        }
    }
}
