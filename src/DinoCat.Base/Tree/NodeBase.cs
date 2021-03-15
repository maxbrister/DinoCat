using DinoCat.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Tree
{
    public abstract class NodeBase<TElement>: Node where TElement: Element
    {
        public NodeBase(int depth, Context context, TElement element) : base(depth, context)
        {
            Element = element;
        }

        public TElement Element { get; private set; }
        public override Element RawElement => Element;

        public sealed override void SetElement(Element newElement)
        {
            var oldElement = Element;
            Element = (TElement)newElement;
            UpdateElement(oldElement);
        }

        protected abstract void UpdateElement(TElement oldElement);
    }
}
