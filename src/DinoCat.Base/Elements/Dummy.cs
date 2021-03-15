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
        public override Node CreateNode(int depth, Context context) =>
            new DummyNode(depth, context, this);
    }

    internal class DummyNode : NodeBase<Dummy>
    {
        public DummyNode(int depth, Context context, Dummy dummy) : base(depth, context, dummy)
        {
        }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        protected override Size ArrangeOverride(Size availableSize) => new Size();


        protected override void UpdateElement(Dummy oldElement) { }

        protected override void UpdateContextOverride(Context oldContext) { }
    }
}
