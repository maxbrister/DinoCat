using DinoCat.State;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class ScopeElement : Element
    {
        public ScopeElement(Func<Element> child) => this.Child = child;

        public Func<Element> Child { get; }

        public override Node CreateNode(Node? parent, Context context) => new ScopeNode(parent, context, this);
    }

    public class ScopeNode : NodeBase<ScopeElement>
    {
        private ImplicitStateScope<Element> scope;
        private Node child;

        public ScopeNode(Node? parent, Context context, ScopeElement element) : base(parent, context, element)
        {
            scope = new(element.Child);
            scope.PropertyChanged += Scope_PropertyChanged;

            var childElement = scope.Execute;
            child = childElement.CreateNode(this, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override (Size, float?) ArrangeOverride(Size availableSize) => child.Arrange(availableSize);

        protected override void UpdateElement(ScopeElement oldElement, Context oldContext)
        {
            scope.Dispose();
            scope = new(Element.Child);
            UpdateState();
        }

        private void UpdateState()
        {
            var newChild = scope.Execute;
            child = child.UpdateElement(newChild, Context);
        }

        private void Scope_PropertyChanged(object? sender, PropertyChangedEventArgs e) =>
            Context.InvalidateState(Depth, UpdateState);
    }
}
