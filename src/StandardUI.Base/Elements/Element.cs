using Microsoft.StandardUI.Tree;
using static Microsoft.StandardUI.Elements.Factories;

namespace Microsoft.StandardUI.Elements
{
    public abstract class Element
    {
        public static implicit operator Element(string text) => Text(text);

        public abstract Node CreateNode(Node? parent, Context context);
    }
}
