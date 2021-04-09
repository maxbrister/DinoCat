using Microsoft.StandardUI.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.StandardUI.Elements
{
    public abstract class Container<TChild> : Element
    {
        public Container(IReadOnlyList<TChild> children) => Children = children;
        public virtual string DescriptionSeparator { get; } = " ";

        public IReadOnlyList<TChild> Children { get; }

        public abstract (Size, float?) Arrange(Context context, Size availableSize, List<Node> children);

        public override Node CreateNode(Node? parent, Context context) => new ContainerNode<TChild>(parent, context, this);

        public abstract bool IsLayoutInvalid(Container<TChild> oldContainer);
        public abstract Element ToElement(TChild child);
    }

    internal class ContainerNode<TChild> : NodeBase<Container<TChild>>
    {
        private List<Node> children;

        public ContainerNode(Node? parent, Context context, Container<TChild> container) : base(parent, context, container)
        {
            children = Element.Children
                .Select(container.ToElement)
                .Select(c => c.CreateNode(this, context)).ToList();
        }

        public override IEnumerable<Node> Children => children;

        public override string Description =>
            string.Join(Element.DescriptionSeparator, Children.Select(c => c.Description).Where(s => !string.IsNullOrEmpty(s)));

        protected override (Size, float?) ArrangeOverride(Size availableSize) =>
            Element.Arrange(Context, availableSize, children);

        protected override void UpdateElement(Container<TChild> oldContainer, Context? oldContext)
        {
            var newCount = Element.Children.Count;
            var updateCount = Math.Min(children.Count, newCount);
            if (children.Count < newCount)
            {
                Context.InvalidateLayout();
                for (int i = children.Count; i < newCount; ++i)
                {
                    var child = Element.ToElement(Element.Children[i]);
                    children.Add(child.CreateNode(this, Context));
                }
            }
            else if (children.Count > newCount)
            {
                Context.InvalidateLayout();
                for (int i = newCount; i < children.Count; ++i)
                    children[i].Dispose();
                children.RemoveRange(newCount, children.Count - newCount);
            }
            else if (Element.IsLayoutInvalid(oldContainer))
            {
                Context.InvalidateLayout();
            }

            for (int i = 0; i < newCount; ++i)
            {
                var node = children[i];
                var child = Element.ToElement(Element.Children[i]);
                children[i] = node.UpdateElement(child, Context);
            }
        }
    }
}
