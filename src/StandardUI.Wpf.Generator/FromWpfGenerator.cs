using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.StandardUI.Wpf.Generator
{
    [Generator]
    public class FromWpfGenerator : ISourceGenerator
    {

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                DoExecute(context);
            }
            catch (TypeNotFoundException e)
            {
                // TODO localize and come up with a real code
                var diagnostic = Diagnostic.Create(
                    "DINO404", "StandardUI", $"Failed to find type {e.QualifiedTypeName}",
                    DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void DoExecute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            bool generatePlatformTypes = compilation.AssemblyName == "StandardUI.Wpf";

            Types types = new(compilation);

#pragma warning disable RS1024 // Compare symbols correctly
            Dictionary<INamedTypeSymbol, string> translated = new(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
            translated[types.DependencyObject] = "Microsoft.StandardUI.Wpf.System.Windows.DependencyObjectBase";
            TranslateTypes translate = new(context, translated, types, generatePlatformTypes);
            if (generatePlatformTypes)
            {
                translate.NamespacePrefix = "Microsoft.StandardUI.Wpf.";
                types.Button.ContainingAssembly.Accept(translate);
                types.UserControl.ContainingAssembly.Accept(translate);
            }
            else
            {
                translate.NamespacePostfix = "Impl";
                compilation.Assembly.Accept(translate);
            }

            translate.WriteFactories();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        class TranslateTypes : SymbolVisitor
        {
            GeneratorExecutionContext context;
            Dictionary<INamedTypeSymbol, string> translated;
            Types types;
            bool allowInternal;
            bool generatePlatformTypes;
            Dictionary<string, List<string>> factoryMethods = new();

            public TranslateTypes(GeneratorExecutionContext context,
                Dictionary<INamedTypeSymbol, string> translated,
                Types types,
                bool generatePlatformTypes)
            {
                this.context = context;
                this.translated = translated;
                this.types = types;
                this.generatePlatformTypes = generatePlatformTypes;
            }

            public string NamespacePrefix { get; set; } = "";
            public string NamespacePostfix { get; set; } = "";

            public override void VisitAssembly(IAssemblySymbol symbol)
            {
                allowInternal = symbol.Equals(context.Compilation.Assembly, SymbolEqualityComparer.Default);
                foreach (var member in symbol.GlobalNamespace.GetMembers())
                    member.Accept(this);
                allowInternal = false;
            }

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                foreach (var type in symbol.GetTypeMembers())
                    type.Accept(this);

                foreach (var ns in symbol.GetNamespaceMembers())
                    ns.Accept(this);
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                if (symbol.IsSubclassOf(types.UIElement) && IsDefaultConstructable(symbol) &&
                    (generatePlatformTypes || symbol.GetAttributes().Any(types.IsFromWpfTypeAttribute)))
                    Translate(symbol);
            }

            public void WriteFactories()
            {
                foreach (var pair in factoryMethods)
                {
                    StringBuilder source = new();
                    source.AppendLine($@"namespace {pair.Key}
{{
    public static partial class Factories
    {{");
                    foreach (var method in pair.Value)
                    {
                        source.Append(' ', 8);
                        source.AppendLine(method);
                    }
                    source.AppendLine("    }");
                    source.Append('}');
                    context.AddSource($"{pair.Key}.Factories.cs", source.ToString());
                }
            }

            private bool IsDefaultConstructable(INamedTypeSymbol symbol) =>
                IsAccessible(symbol.DeclaredAccessibility) &&
                !symbol.IsGenericType &&
                !symbol.IsAbstract &&
                symbol.InstanceConstructors.Any(c =>
                    c.Parameters.IsEmpty &&
                    IsAccessible(c.DeclaredAccessibility) &&
                    !c.IsStatic);


            private bool IsAccessible(Accessibility access) =>
                access == Accessibility.Public || (allowInternal && access == Accessibility.Internal);

            private string Translate(INamedTypeSymbol wpfType)
            {
                if (translated.TryGetValue(wpfType, out var v))
                    return v;

                var qualifiedWpfType = wpfType.ToDisplayString();
                if (!generatePlatformTypes && qualifiedWpfType.StartsWith("System.Windows"))
                {
                    var dinoTypeName = $"global::Microsoft.StandardUI.Wpf.{qualifiedWpfType}Base";
                    translated[wpfType] = dinoTypeName;
                    return dinoTypeName;
                }

                qualifiedWpfType = $"global::{qualifiedWpfType}";

                var name = wpfType.Name;
                string dinoNamespace;
                string dinoType;
                var attributeData = wpfType.GetAttributes().FirstOrDefault(types.IsFromWpfTypeAttribute);
                if (attributeData != null)
                {
                    dinoType = (string)attributeData.ConstructorArguments[0].Value!;
                    int split = dinoType.LastIndexOf('.');
                    if (split == -1)
                    {
                        // TODO report error
                    }
                    else if (split == dinoType.Length)
                    {
                        // TODO report error
                    }
                    dinoNamespace = dinoType.Substring(0, split);
                    name = dinoType.Substring(split + 1);
                }
                else
                {
                    dinoNamespace = NamespacePrefix +
                        wpfType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Substring("global::".Length);
                    dinoType = dinoNamespace + "." + name;
                }
                var dinoBaseType = dinoType + "Base";
                translated[wpfType] = dinoBaseType;

                var wpfBaseType = wpfType.BaseType;
                if (wpfBaseType == null)
                {
                    context.PrintDebug($"Failed to find base type for {wpfType.Name}");
                    return string.Empty;
                }

                var access = wpfType.DeclaredAccessibility.Format();
                var basedOn = Translate(wpfBaseType);
                StringBuilder source = new();
                source.AppendLine($@"#pragma warning disable CS0108
#pragma warning disable CS8604
namespace {dinoNamespace}
{{");
                if (!wpfType.IsSealed)
                {
                    source.AppendLine($@"
    {access} abstract partial class {name}Base<TSubclass, TWpf> : {basedOn}<TSubclass, TWpf> where TWpf : {qualifiedWpfType}, new()
    {{");
                    WriteConstructors(source, name + "Base", "TWpf");
                    WriteBase(source, wpfType, qualifiedWpfType, "TWpf", "TSubclass");
                    source.AppendLine("    }");
                }


                if (IsDefaultConstructable(wpfType) &&
                    (wpfType.IsSubclassOf(types.UIElement) || wpfType.Equals(types.UIElement, SymbolEqualityComparer.IncludeNullability)))
                {
                    string parent = wpfType.IsSealed ? basedOn : name + "Base";
                    source.AppendLine($@"
    {access} sealed partial class {name} : {parent}<{name}, {qualifiedWpfType}>
    {{");
                    WriteConstructors(source, name, qualifiedWpfType);
                    if (wpfType.IsSealed)
                        WriteBase(source, wpfType, qualifiedWpfType, qualifiedWpfType, name);
                    WriteReal(source, name, qualifiedWpfType);
                    source.AppendLine("    }");

                    if (!factoryMethods.TryGetValue(dinoNamespace, out var methods))
                    {
                        methods = new();
                        factoryMethods[dinoNamespace] = methods;
                    }
                    methods.Add($"{access} static global::{dinoNamespace}.{name} {name}() => new();");
                }
                source.Append("}");

                var sourceFile = $"{dinoType}.cs";
                context.AddSource(sourceFile, source.ToString());

                // I can't find a way to see the generator output. Just write it to a file for debugging.
                if (generatePlatformTypes)
                {
                    //Directory.CreateDirectory("C:\\DinoCat\\src\\DinoCat.Wpf\\Microsoft.StandardUI.Wpf.Generator\\Microsoft.StandardUI.Wpf.Generator.FromWpfGenerator");
                    //File.WriteAllText($"C:\\DinoCat\\src\\DinoCat.Wpf\\Microsoft.StandardUI.Wpf.Generator\\Microsoft.StandardUI.Wpf.Generator.FromWpfGenerator\\{sourceFile}.cs", source.ToString());
                }
                else
                {
                    //Directory.CreateDirectory("C:\\DinoCat\\examples\\Interop.Wpf\\Microsoft.StandardUI.Wpf.Generator\\Microsoft.StandardUI.Wpf.Generator.FromWpfGenerator");
                    //File.WriteAllText($"\\DinoCat\\examples\\Interop.Wpf\\Microsoft.StandardUI.Wpf.Generator\\Microsoft.StandardUI.Wpf.Generator.FromWpfGenerator\\{sourceFile}.cs", source.ToString());
                }

                return dinoBaseType;
            }

            private void WriteConstructors(StringBuilder source, string name, string qualifiedWpfType)
            {
                source.AppendLine($@"
        public {name}() {{}}
        public {name}((global::System.Windows.DependencyProperty, object?)[] localValues, global::Microsoft.StandardUI.Wpf.System.Windows.Internal.Operation<{qualifiedWpfType}>?[] operations) : base(localValues, operations)
        {{}}
        public {name}(
            global::System.Collections.Immutable.ImmutableDictionary<global::System.Windows.DependencyProperty, object> localValues,
            global::System.Collections.Immutable.ImmutableList<global::Microsoft.StandardUI.Wpf.System.Windows.Internal.Operation<{qualifiedWpfType}>> operations) : base(localValues, operations)
        {{}}
");
            }

            private void WriteBase(StringBuilder source, INamedTypeSymbol wpfType, string qualifedWpfType, string tWpf, string meType)
            {
                Dictionary<string, (INamedTypeSymbol, Accessibility)> properties = new();
                foreach (var property in wpfType.GetMembers().OfType<IPropertySymbol>())
                {
                    if (property.IsStatic || property.IsIndexer)
                        continue;

                    var accessibility = property.GetMethod?.DeclaredAccessibility ??
                        property.SetMethod?.DeclaredAccessibility ?? Accessibility.Private;
                    if (IsAccessible(accessibility) && property.Type is INamedTypeSymbol namedType)
                        properties[property.Name] = (namedType, accessibility);
                }

                foreach (var member in wpfType.GetMembers())
                {
                    if (!IsAccessible(member.DeclaredAccessibility))
                        continue;

                    ITypeSymbol? fieldType = null;
                    if (member.IsStatic && member is IPropertySymbol property && !property.IsWriteOnly && !property.IsWithEvents)
                        fieldType = property.Type;
                    else if (member.IsStatic && member is IFieldSymbol field)
                        fieldType = field.Type;
                    else if (!member.IsStatic && member is IEventSymbol evt)
                    {
                        var accessibility = evt.DeclaredAccessibility.Format();
                        string typeName;
                        if (evt.Type is INamedTypeSymbol eventType &&
                            eventType.DelegateInvokeMethod?.Parameters.Length == 2)
                            typeName = eventType.DelegateInvokeMethod.Parameters[1].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        else
                            continue;

                        source.Append(' ', 8);
                        source.Append($@"{accessibility} {meType} On{evt.Name}(global::System.Action<{typeName}> callback)
        {{
            void OnEvent(object? sender, {typeName} args)
            {{
                callback(args);
            }}
            return NewImpl(LocalValues, Operations.Add(new global::Microsoft.StandardUI.Wpf.System.Windows.Internal.Operation<{tWpf}>
            {{
                Apply = element => element.{evt.Name} += OnEvent,
                Unapply = element => element.{evt.Name} -= OnEvent
            }}));
        }}");
                        continue;
                    }
                    else
                        continue;

                    bool isDependencyProperty = fieldType.Equals(types.DependencyProperty, SymbolEqualityComparer.Default);
                    bool isRoutedEvent = !isDependencyProperty && fieldType.Equals(types.RoutedEvent, SymbolEqualityComparer.Default);
                    if (isDependencyProperty || isRoutedEvent)
                        TransferField(source, qualifedWpfType, fieldType, member);
                    else
                        continue;

                    const string propertySuffix = "Property";
                    var fieldName = member.Name;
                    if (isDependencyProperty && fieldName.EndsWith("Property"))
                        fieldName = fieldName.Substring(0, fieldName.Length - propertySuffix.Length);

                    if (isDependencyProperty && properties.TryGetValue(fieldName, out var pair))
                    {
                        var propertyType = pair.Item1;
                        var access = pair.Item2;
                        string template = $"        {access.Format()} {meType} {fieldName}";
                        source.AppendLine($@"{template}({propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value) =>
            Set({member.Name}, value);");

                        if (fieldType.GetDinoType() is string dinoTypeName)
                        {
                            source.AppendLine($@"{template}(global::{dinoTypeName} value) =>
            Set({member.Name}, value);");
                        }
                    }
                }
            }

            private void TransferField(StringBuilder source, string qualifiedWpfType, ITypeSymbol fieldType, ISymbol field)
            {
                var typeName = fieldType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var access = field.DeclaredAccessibility.Format();
                source.Append(new string(' ', 8));
                source.AppendLine($"{access} static readonly {typeName} {field.Name} = {qualifiedWpfType}.{field.Name};");
            }

            private void WriteReal(StringBuilder source, string name, string qualifiedWpfType)
            {
                source.AppendLine($@"
        protected override {name} NewImpl(
            global::System.Collections.Immutable.ImmutableDictionary<global::System.Windows.DependencyProperty, object> localValues,
            global::System.Collections.Immutable.ImmutableList<global::Microsoft.StandardUI.Wpf.System.Windows.Internal.Operation<{qualifiedWpfType}>> operations) =>
            new(localValues, operations);
");
            }
        }
    }
}
