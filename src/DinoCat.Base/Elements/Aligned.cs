﻿using DinoCat.Drawing;
using DinoCat.Tree;
using System.Collections.Generic;

namespace DinoCat.Elements
{
    public class Aligned
    {
        public Aligned(Element child, float? x = null, float? y = null)
        {
            Child = child;
            X = x;
            Y = y;
        }

        public static implicit operator Aligned(Element child) => new Aligned(child);

        public Element Child { get; }
        public float? X { get; }
        public float? Y { get; }

        public void AlignNode(Node node, Size slot) =>
            AlignNode(node, slot, X, Y);

        public static void AlignNode(Node node, Size slot, float? x, float? y)
        {
            var offset = new Point();
            var size = node.Size;
            if (!float.IsInfinity(slot.Width) && size.Width < slot.Width && x is float xv)
                offset.X = (slot.Width - size.Width) * xv;

            if (!float.IsInfinity(slot.Height) && size.Height < slot.Height && y is float yv)
                offset.Y = (slot.Height - size.Height) * yv;
            node.Offset += offset;
        }
    }

    public static class AlignedExtension
    {
        public static Aligned Align(this Element e, float? x = null, float? y = null) => new Aligned(e, x, y);
        public static Aligned Left(this Element e) => new Aligned(e, 0, null);
        public static Aligned Top(this Element e) => new Aligned(e, null, 0);
        public static Aligned Right(this Element e) => new Aligned(e, 1, null);
        public static Aligned Bottom(this Element e) => new Aligned(e, null, 1);
        public static Aligned TopLeft(this Element e) => new Aligned(e, 0, 0);
        public static Aligned TopRight(this Element e) => new Aligned(e, 1, 0);
        public static Aligned BottomLeft(this Element e) => new Aligned(e, 0, 1);
        public static Aligned BottomRight(this Element e) => new Aligned(e, 1, 1);
        public static Aligned Center(this Element e) => new Aligned(e, 0.5f, 0.5f);
    }
}
