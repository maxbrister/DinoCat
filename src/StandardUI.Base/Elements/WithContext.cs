using Microsoft.StandardUI.Tree;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Elements
{
    public class WithContextElement : Element
    {
        public WithContextElement(Element child, ImmutableDictionary<Type, Func<object>> values, object? id = null)
        {
            Child = child;
            Id = id;
            Values = values;
        }

        public Element Child { get; }
        public object? Id { get; }
        public ImmutableDictionary<Type, Func<object>> Values { get; }

        public Context CreateChildContext(Context parent)
        {
            var evaluated = Values.ToDictionary(pair => pair.Key, pair => pair.Value());
            return new Context(parent, evaluated);
        }

        public override Node CreateNode(Node? parent, Context context) =>
            new WithContextNode(parent, context, this);

        public WithContextElement WithContext<T>(Func<T> create) where T : notnull =>
            new WithContextElement(Child, Values.SetItem(typeof(T), () => create()), Id);
    }

    public static class WithContextExtension
    {
        public static WithContextElement WithContext<T>(this Element element, Func<T> createInstance, object? id = null) where T: notnull =>
            new WithContextElement(element, new Dictionary<Type, Func<object>>
            {
                { typeof(T), () => createInstance() }
            }.ToImmutableDictionary(), id);
    }

    internal class WithContextNode : NodeBase<WithContextElement>
    {
        private Node child;

        public WithContextNode(Node? parent, Context context, WithContextElement element) : base(parent, context, element)
        {
            var childContext = element.CreateChildContext(context);
            child = element.Child.CreateNode(this, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override (Size, float?) ArrangeOverride(Size availableSize) => child.Arrange(availableSize);

        protected override void UpdateElement(WithContextElement oldElement, Context oldContext)
        {
            var childContext = child.Context;
            if (oldContext != Context || !Equals(oldElement.Id, Element.Id))
                childContext = Element.CreateChildContext(Context);
            child = child.UpdateElement(Element.Child, childContext);
        }
    }
}
