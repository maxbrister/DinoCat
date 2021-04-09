using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCat.Wpf.Generator
{
    class Types
    {
        public Types(Compilation compilation)
        {
            Compilation = compilation;
            Button = Load("System.Windows.Controls.Button");
            UserControl = Load("System.Windows.Controls.UserControl");
            UIElement = Load("System.Windows.UIElement");
            RoutedEvent = Load("System.Windows.RoutedEvent");
            DependencyProperty = Load("System.Windows.DependencyProperty");
            DependencyObject = Load("System.Windows.DependencyObject");
            Thickness = Load("System.Windows.Thickness").SetDinoType("Dino.Thickness");
            Brush = Load("System.Windows.Media.Brush").SetDinoType("Dino.Drawing.Brush");
            FromWpfTypeAttribute = Load("DinoCat.Wpf.FromWpfTypeAttribute");
            ToWpfTypeAttribute = Load("DinoCat.Wpf.ToWpfTypeAttribute");
            Element = Load("DinoCat.Elements.Element");
            Action = Load("System.Action");
        }

        public Compilation Compilation { get; }
        public INamedTypeSymbol Button { get; }
        public INamedTypeSymbol UserControl { get; }
        public INamedTypeSymbol UIElement { get; }
        public INamedTypeSymbol RoutedEvent { get; }
        public INamedTypeSymbol DependencyProperty { get; }
        public INamedTypeSymbol DependencyObject { get; }
        public INamedTypeSymbol Thickness { get; }
        public INamedTypeSymbol Brush { get; }
        public INamedTypeSymbol FromWpfTypeAttribute { get; }
        public INamedTypeSymbol ToWpfTypeAttribute { get; }
        public INamedTypeSymbol Element { get; }
        public INamedTypeSymbol Action { get; }

        public bool IsFromWpfTypeAttribute(AttributeData data) =>
            FromWpfTypeAttribute.Equals(data.AttributeClass, SymbolEqualityComparer.IncludeNullability);

        public bool IsToWpfTypeAttribute(AttributeData data) =>
            ToWpfTypeAttribute.Equals(data.AttributeClass, SymbolEqualityComparer.IncludeNullability);

        private INamedTypeSymbol Load(string qualifiedTypeName) =>
            Compilation.GetTypeByMetadataName(qualifiedTypeName) ?? throw new TypeNotFoundException(qualifiedTypeName);
    }
}
