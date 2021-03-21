using DinoCat.Drawing;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Elements
{
    public class Text : Element
    {
        public Text(string text, Brush? foreground = null, ITypeface? typeface = null)
        {
            Content = text;
            Foreground = foreground ?? Colors.Black;
            Typeface = typeface;
        }

        public string Content { get; }
        public Brush Foreground { get; }
        public ITypeface? Typeface { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new TextNode(parent, context, this);
    }

    internal class TextNode : NodeBase<Text>
    {
        private IFormattedText? formatted;

        public TextNode(Node? parent, Context context, Text text) : base(parent, context, text) { }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        public override string Description => Element.Content;

        protected override Size ArrangeOverride(Size availableSize)
        {

            return new Size(Formatted.Width, Formatted.Height);
        }

        protected override void RenderOverride(DrawingContext context)
        {
            context.DrawText(Formatted, new Point());
        }

        protected override void UpdateElement(Text oldElement, Context oldContext)
        {
            formatted = null;
            Context.InvalidateLayout();
            Context.InvalidateRender();
        }

        private IFormattedText Formatted
        {
            get
            {
                if (formatted == null)
                {
                    var typeface = Element.Typeface ?? Context.TryGet<ITypeface>();
                    var fontManager = Context.TryGet<IFontManager>();
                    formatted = fontManager!.FormatText(typeface!, Element.Foreground, 12, Element.Content);
                }

                return formatted;
            }
        }
    }
}
