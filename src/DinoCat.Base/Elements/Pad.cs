using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Pad : Element
    {
        public Pad(Element child, double left = 0, double top = 0, double right = 0, double bottom = 0)
        {
            if (child is Pad childPad)
            {
                child = childPad.Child;
                left += childPad.Left;
                top += childPad.Top;
                right += childPad.Right;
                bottom += childPad.Bottom;
            }

            Child = child;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Element Child { get; }
        public double Left { get; }
        public double Top { get; }
        public double Right { get; }
        public double Bottom { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new PadNode(parent, context, this);
    }

    public static class PadExtensions
    {
        public static Pad Pad(this Element e, double left = 0, double top = 0, double right = 0, double bottom = 0) =>
            new Pad(e, left, top, right, bottom);
        public static Pad PadUniform(this Element e, double pad) =>
            new Pad(e, pad, pad, pad, pad);
    }

    internal class PadNode : NodeBase<Pad>
    {
        private Node child;

        public PadNode(Node? parent, Context context, Pad pad) : base(parent, context, pad)
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
            var pad = Element;
            var hpad = pad.Left + pad.Right;
            var vpad = pad.Top + pad.Bottom;
            var childAvailable = new Size(
                Math.Max(0, availableSize.Width - hpad),
                Math.Max(0, availableSize.Height - vpad));
            var childSize = child.Arrange(childAvailable);
            child.Offset = new Point(pad.Left, pad.Top);
            return new Size(childSize.Width + hpad, childSize.Height + vpad);
        }

        protected override void UpdateElement(Pad oldPad, Context oldContext)
        {
            var newPad = Element;
            if (oldPad.Left != newPad.Left ||
                oldPad.Top != newPad.Top ||
                oldPad.Right != newPad.Right ||
                oldPad.Bottom != newPad.Bottom)
                Context.InvalidateLayout();

            child = child.UpdateElement(newPad.Child, Context);
        }
    }
}
