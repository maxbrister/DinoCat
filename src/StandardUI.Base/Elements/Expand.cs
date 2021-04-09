using DinoCat.Tree;
using System.Collections.Generic;

namespace DinoCat.Elements
{
    /// <summary>
    /// Take up remaining space in a <see cref="Row"/>, <see cref="Column"/>, or <see cref="Stack"/>.
    /// </summary>
    /// <remarks><see cref="Row"/> and <see cref="Column"/> split their remaing space based on the <see cref="Factor"/>.
    /// For <see cref="Stack"/> any non-zero factor fills the entire stack.</remarks>
    /// <param name="factor">The amount of space to take up relative to other expanded elements. 0 indicates the element should not be expanded.</param>
    public class Expand
    {
        public Expand(Element child, int factor = 1)
        {
            Child = child;
            Factor = factor;
        }

        public static implicit operator Expand(Element e) => new(e, factor: 0);

        public Element Child { get; }
        public int Factor { get; }
    }

    public static class ExpandHelper
    {
        public static Expand Expand(this Element e, int factor = 1) => new(e, factor);
    }
}
