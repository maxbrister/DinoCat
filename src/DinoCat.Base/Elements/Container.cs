﻿using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public abstract class Container : Element
    {
        public Container(IReadOnlyList<Element> children) => Children = children;
        public virtual string DescriptionSeparator { get; } = " ";

        public IReadOnlyList<Element> Children { get; }

        public abstract Size Arrange(Size availableSize, List<Node> children);

        public override Node CreateNode(Node? parent, Context context) => new ContainerNode(parent, context, this);

        public abstract bool IsLayoutInvalid(Container oldContainer);
    }

    internal class ContainerNode : NodeBase<Container>
    {
        private List<Node> children;

        public ContainerNode(Node? parent, Context context, Container container) : base(parent, context, container)
        {
            children = Element.Children.Select(c => c.CreateNode(this, context)).ToList();
        }

        public override IEnumerable<Node> Children => children;

        public override string Description =>
            string.Join(Element.DescriptionSeparator, Children.Select(c => c.Description).Where(s => !string.IsNullOrEmpty(s)));

        protected override Size ArrangeOverride(Size availableSize) =>
            Element.Arrange(availableSize, children);

        protected override void UpdateElement(Container oldContainer, Context? oldContext)
        {
            var newCount = Element.Children.Count;
            var updateCount = Math.Min(children.Count, newCount);
            if (children.Count < newCount)
            {
                Context.InvalidateLayout();
                for (int i = children.Count; i < newCount; ++i)
                {
                    children.Add(Element.Children[i].CreateNode(this, Context));
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
                children[i] = node.UpdateElement(Element.Children[i], Context);
            }
        }
    }
}
