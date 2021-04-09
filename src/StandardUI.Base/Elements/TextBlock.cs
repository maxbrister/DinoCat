using Microsoft.StandardUI.Drawing;
using Microsoft.StandardUI.Tree;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.StandardUI.Elements
{
    public class TextBlock : Element
    {
        public TextBlock(string text, Brush? foreground = null, float? fontSize = null, ITypeface? typeface = null)
        {
            Content = text;
            Foreground = foreground;
            Typeface = typeface;
            FontSize = fontSize;
        }

        public string Content { get; }
        public Brush? Foreground { get; }
        public ITypeface? Typeface { get; }
        public float? FontSize { get; }

        public override Node CreateNode(Node? parent, Context context) =>
            new TextBlockNode(parent, context, this);
    }

    internal class TextBlockNode : NodeBase<TextBlock>
    {
        private IFormattedText? formatted;

        public TextBlockNode(Node? parent, Context context, TextBlock text) : base(parent, context, text) { }

        public override IEnumerable<Node> Children => Enumerable.Empty<Node>();

        public override string Description => Element.Content;

        public IFormattedText Formatted
        {
            get
            {
                if (formatted == null)
                {
                    var theme = Context.Get<TextTheme>();
                    var fontManager = Context.Get<IFontManager>();
                    formatted = fontManager.FormatText(
                        Element.Typeface ?? theme.Typeface,
                        Element.Foreground ?? theme.Foreground,
                        Element.FontSize ?? theme.FontSize,
                        Context.Get<DpiScale>(),
                        Context.Get<FlowDirection>(),
                        Element.Content);
                }

                return formatted;
            }
        }

        protected override (Size, float?) ArrangeOverride(Size availableSize)
        {
            return (new Size(Formatted.Width, Formatted.Height), -Formatted.Ascent);
        }

        protected override void RenderOverride(DrawingContext context)
        {
            context.DrawText(Formatted, new Point());
        }

        protected override void UpdateElement(TextBlock oldElement, Context oldContext)
        {
            formatted = null;
            Context.InvalidateLayout();
            Context.InvalidateRender();
        }
    }
}
