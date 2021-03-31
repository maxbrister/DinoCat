using DinoCat.Tree;
using static DinoCat.Elements.Factories;

namespace DinoCat.Elements
{
    public abstract class Element
    {
        public static implicit operator Element(string text) => Text(text);

        public abstract Node CreateNode(Node? parent, Context context);
    }
}
