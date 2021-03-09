using DinoCat.Base.Tree;
using System;

namespace DinoCat.Base.Elements
{
    public abstract class Control : Element
    {
        public abstract Element Build(BuildContext context);

        Node Element.CreateNode(int depth, BuildContext context)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Control<TState> : Element where TState: IState, new()
    {
        public abstract Element Build(BuildContext context, TState state);

        Node Element.CreateNode(int depth, BuildContext context)
        {
            throw new NotImplementedException();
        }
    }
}
