using DinoCat.Elements;
using DinoCat.Tree;
using System;
using System.Windows;
using System.Windows.Controls;
using Control = DinoCat.Elements.Control;
using ContentControl = DinoCat.Wpf.System.Windows.Controls.ContentControl;

namespace DinoCat.Wpf
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
                    return new DinoCat.Elements.TextBlock(s);

                return new ContentControl().Content(Content);
            }
        }

        public override Element Build(Context context) => Child;
    }
}
