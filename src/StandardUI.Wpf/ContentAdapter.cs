using Microsoft.StandardUI.Elements;
using Microsoft.StandardUI.Tree;
using System;
using System.Windows;
using System.Windows.Controls;
using Control = Microsoft.StandardUI.Elements.Control;
using ContentControl = Microsoft.StandardUI.Wpf.System.Windows.Controls.ContentControl;

namespace Microsoft.StandardUI.Wpf
{
    public class ContentAdapter : Control
    {
        public ContentAdapter(object content) => Content = content;
        public object Content { get; }

        public Element Child
        {
            get
            {
                if (Content is Element dinoChild)
                    return dinoChild;
                else if (Content is UIElement wpfChild)
                    return new RawUIElement(wpfChild);
                else if (Content is string s)
                    return new Microsoft.StandardUI.Elements.TextBlock(s);

                return new ContentControl().Content(Content);
            }
        }

        public override Element Build(Context context) => Child;
    }
}
