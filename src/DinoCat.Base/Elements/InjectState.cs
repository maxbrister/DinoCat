using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class InjectState<TState> : Element where TState: IState
    {
        public InjectState(Func<TState> newState, Func<TState, Element> callback)
        {
            NewState = newState;
            Callback = callback;
        }

        public Func<TState> NewState { get; }
        public Func<TState, Element> Callback { get; }

        public virtual bool Safe { get; } = true;

        public override Node CreateNode(int depth, Context context) =>
            new InjectStateNode<TState>(depth, context, this);
    }

    public class UnsafeInjectState<TState> : InjectState<TState> where TState : IState
    {
        public UnsafeInjectState(Func<TState> newState, Func<TState, Element> callback) : base(newState, callback)
        { }

        public override bool Safe { get; } = false;
    }

    internal class InjectStateNode<TState> : NodeBase<InjectState<TState>> where TState: IState
    {
        private TState state;
        private Node child;


        public InjectStateNode(int parentDepth, Context context, InjectState<TState> element) : base(parentDepth, context, element)
        {
            state = element.NewState();

            if (element.Safe)
                state.StateChanged += State_StateChanged;

            var childElement = element.Callback(state);
            child = childElement.CreateNode(parentDepth, context);
        }

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        protected override Size ArrangeOverride(Size availableSize) => child.Arrange(availableSize);

        protected override void UpdateElement(InjectState<TState> oldElement) => UpdateState();

        protected override void UpdateContextOverride(Context oldContext)
        {
            var newChild = Element.Callback(state);
            child = child.UpdateElement(newChild, Context);
        }

        private void UpdateState()
        {
            var newChild = Element.Callback(state);
            child = child.UpdateElement(newChild);
        }

        private void State_StateChanged(object? sender, EventArgs e) =>
            Context.InvalidateState(Depth, UpdateState);
    }
}
