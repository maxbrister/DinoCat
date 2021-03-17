using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Dummy : Element
    {
        public override Node CreateNode(Node? parent, Context context) =>
            new DummyNode(parent, context, this);
    }

    internal class DummyNode : NodeBase<Dummy>
    {
        public DummyNode(Node? parent, Context context, Dummy dummy) : base(parent, context, dummy)
        {
        }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        protected override Size ArrangeOverride(Size availableSize) => new Size();


        protected override void UpdateElement(Dummy oldElement, Context oldContext) { }
    }
}
