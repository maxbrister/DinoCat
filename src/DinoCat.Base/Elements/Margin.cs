﻿using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Margin : Element
    {
        public Margin(Element child, Thickness value)
        {
            if (child is Margin childMargin)
            {
                child = childMargin.Child;
                value += childMargin.Value;
            }

            Child = child;
            Value = value;
        }

        public Element Child { get; }
        public Thickness Value { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new PadNode(parent, context, this);
    }

    public static class MarginExtensions
    {
        public static Margin Margin(this Element e, float uniform) =>
            new Margin(e, new(uniform));
        public static Margin Margin(this Element e, float left, float top) =>
            new Margin(e, new(left, top));
        public static Margin Margin(this Element e, float left, float top, float right, float bottom) =>
            new Margin(e, new(left, right, top, bottom));
        public static Margin Margin(this Element e, Thickness margin) =>
            new Margin(e, margin);
    }

    internal class PadNode : NodeBase<Margin>
    {
        private Node child;

        public PadNode(Node? parent, Context context, Margin pad) : base(parent, context, pad)
        {
            child = pad.Child.CreateNode(this, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            var margin = Element.Value;
            var hmargin = margin.Left + margin.Right;
            var vmargin = margin.Top + margin.Bottom;
            var childAvailable = new Size(
                Math.Max(0, availableSize.Width - hmargin),
                Math.Max(0, availableSize.Height - vmargin));
            var childSize = child.Arrange(childAvailable);
            child.Offset = new Point(margin.Left, margin.Top);
            return new Size(childSize.Width + hmargin, childSize.Height + vmargin);
        }

        protected override void UpdateElement(Margin oldMargin, Context oldContext)
        {
            if (oldMargin.Value != Element.Value)
                Context.InvalidateLayout();

            child = child.UpdateElement(Element.Child, Context);
        }
    }
}
