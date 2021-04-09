namespace Microsoft.StandardUI.Drawing
{
    // FontManager (and FormattedText) are still in very early stages. A lot more effort needs
    // to be done before we have production quailty text layout/rendering.
    //
    // TODO:
    // * RTL support
    // * Multiline support
    // * Runs
    // * Allow for emebeded UI elements (forced space)?
    public interface IFontManager
    {
        ITypeface CreateTypeface(string fontName);
        ITypeface CreateTypeface(string fontName, FontSlant slant, int weight);
        IFormattedText FormatText(ITypeface typeface, Brush brush, float fontPt, DpiScale scale, string text);
    }
}
