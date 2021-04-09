using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Tree;
using System;
using System.Collections.Generic;

namespace Microsoft.StandardUI.Elements
{
    public class Aligned : Control
    {
        public Aligned(Element child, HorizontalAlignment? horizontal, VerticalAlignment? vertical)
        {
            if (child is Aligned aligned)
            {
                Child = aligned.Child;
                Horizontal = horizontal ?? aligned.Horizontal;
                Vertical = vertical ?? aligned.Vertical;
            }
            else
            {
                Child = child;
                Horizontal = horizontal;
                Vertical = vertical;
            }
        }

        public Element Child { get; }
        public HorizontalAlignment? Horizontal { get; }
        public VerticalAlignment? Vertical { get; }

        public override Element Build(Context context) => Child;

        public static HorizontalAlignment GetHorizontalAlignment(Element element, HorizontalAlignment defaultAlignment) =>
            (element as Aligned)?.Horizontal ?? defaultAlignment;

        public static VerticalAlignment GetVerticalAlignment(Element element, VerticalAlignment defaultAlignment) =>
            (element as Aligned)?.Vertical ?? defaultAlignment;

        public static void Align(
            Node node, DpiScale scale, FlowDirection flowDirection,
            float baseline, float remainingWidth, float remainingHeight,
            HorizontalAlignment defaultHorizontal, VerticalAlignment defaultVertical)
        {
            var aligned = node.RawElement as Aligned;
            var offset = node.Offset;
            if (!float.IsInfinity(remainingWidth) && remainingWidth > 0)
            {
                var horizontal = aligned?.Horizontal ?? defaultHorizontal;
                float alignment = 0;
                switch (horizontal)
                {
                    case HorizontalAlignment.Center:
                        alignment = 0.5f;
                        break;
                    case HorizontalAlignment.Right:
                        alignment = 1;
                        break;
                }

                if (flowDirection == FlowDirection.RightToLeft)
                    alignment = 1 - alignment;

                offset.X += (float)Math.Ceiling(alignment * remainingWidth * scale.X) / scale.X;
            }

            if (!float.IsInfinity(remainingHeight) && remainingHeight > 0)
            {
                var vertical = aligned?.Vertical ?? defaultVertical;
                if (vertical == VerticalAlignment.Baseline)
                {
                    float nodeBaseline = node.Baseline ?? node.Size.Height;
                    offset.Y = baseline - nodeBaseline;
                }
                else
                {
                    float alignment = 0;
                    switch (vertical)
                    {
                        case VerticalAlignment.Center:
                            alignment = 0.5f;
                            break;
                        case VerticalAlignment.Bottom:
                            alignment = 1;
                            break;
                    }
                    offset.Y = (float)Math.Ceiling(alignment * remainingHeight * scale.Y) / scale.Y;
                }
            }

            node.Offset = offset;
        }
    }

    public static class AlignedExtension
    {
        public static Aligned Align(this Element e, HorizontalAlignment horizontal, VerticalAlignment vertical) => new(e, horizontal, vertical);
        public static Aligned Left(this Element e) => new(e, HorizontalAlignment.Left, null);
        public static Aligned Top(this Element e) => new(e, null, VerticalAlignment.Top);
        public static Aligned Right(this Element e) => new(e, HorizontalAlignment.Right, null);
        public static Aligned Bottom(this Element e) => new(e, null, VerticalAlignment.Bottom);
        public static Aligned TopLeft(this Element e) => new(e, HorizontalAlignment.Left, VerticalAlignment.Top);
        public static Aligned TopRight(this Element e) => new(e, HorizontalAlignment.Right, VerticalAlignment.Top);
        public static Aligned BottomLeft(this Element e) => new(e, HorizontalAlignment.Left, VerticalAlignment.Bottom);
        public static Aligned BottomRight(this Element e) => new(e, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        public static Aligned Center(this Element e) => new(e, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
