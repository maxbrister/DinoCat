using DinoCat.Base.Drawing;
using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Elements
{
    public class Dummy : Element
    {
        public Node CreateNode(int depth, BuildContext context) =>
            new DummyNode(depth, context);
    }

    internal class DummyNode : Node
    {
        public DummyNode(int depth, BuildContext context) : base(depth, context)
        {
        }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        public override Size Arrange(Size availableSize) => new Size();

        public override void Dispose() { }

        public override IEnumerable<(Node, Point)> HitTest(Point p) =>
            Enumerable.Empty<(Node, Point)>();

        public override void Render(IDrawingContext context) { }

        public override void UpdateElement(Element newElement) { }

        public override void UpdateState() { }
    }
}
