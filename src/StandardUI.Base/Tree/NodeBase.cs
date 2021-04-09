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
        public NodeBase(Node? parent, Context context, TElement element) : base(parent, context)
        {
            Element = element;
        }

        public TElement Element { get; private set; }
        public override Element RawElement => Element;

        protected sealed override void SetElement(Element newElement, Context oldContext)
        {
            var oldElement = Element;
            Element = (TElement)newElement;
            UpdateElement(oldElement, oldContext);
        }

        protected abstract void UpdateElement(TElement oldElement, Context oldContext);
    }
}
