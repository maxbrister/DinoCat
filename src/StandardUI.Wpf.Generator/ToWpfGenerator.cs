using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DinoCat.Wpf.Generator
{
    [Generator]
    public class ToWpfGenerator : ISourceGenerator
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
                    "DINO404", "DinoCat", $"Failed to find type {e.QualifiedTypeName}",
                    DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void DoExecute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            bool generatePlatformTypes = compilation.AssemblyName == "DinoCat.Wpf";
            Types types = new(compilation);
            TranslateTypes translate = new(context, types, generatePlatformTypes);
            if (generatePlatformTypes)
            {
                var dinoBase = types.Element.ContainingAssembly;
                dinoBase.Accept(translate);
            }
            else
                compilation.Assembly.Accept(translate);
        }

        public void Initialize(GeneratorInitializationContext context) { }

        private class TranslateTypes : SymbolVisitor
        {
            GeneratorExecutionContext context;
            Types types;
            bool generatePlatformTypes;
            bool allowInternal;
            Dictionary<INamedTypeSymbol, (string, string)> mappedTypes;

            public TranslateTypes(GeneratorExecutionContext context, Types types, bool generatePlatformTypes)
            {
                this.context = context;
                this.types = types;
                this.generatePlatformTypes = generatePlatformTypes;
#pragma warning disable RS1024 // Compare symbols correctly
                mappedTypes = new(SymbolEqualityComparer.Default)
                {
                    { types.Element, ("object", "new ContentAdapter({0})") },
                    { types.Action, ("", "() => {0}?.Invoke(this, global::System.EventArgs.Empty)") }
                };
#pragma warning restore RS1024 // Compare symbols correctly
            }

            public override void VisitAssembly(IAssemblySymbol symbol)
            {
                allowInternal = symbol.Equals(context.Compilation.Assembly, SymbolEqualityComparer.Default);
                foreach (var member in symbol.GlobalNamespace.GetMembers())
                    member.Accept(this);
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
                if (!IsAccessible(symbol.DeclaredAccessibility) || symbol.IsGenericType ||
                    symbol.IsAbstract || !symbol.IsSubclassOf(types.Element))
                    return;

                var toWpf = symbol.GetAttributes().FirstOrDefault(types.IsToWpfTypeAttribute);
                if (!generatePlatformTypes && toWpf == null)
                    return;

                const string platformPrefix = "global::DinoCat.Elements.";
                var originalQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                string projectedNamespace;
                string projectedTypeName;
                if (toWpf != null)
                {
                    var projected = (string)toWpf.ConstructorArguments[0].Value!;
                    int split = projected.LastIndexOf('.');
                    // TODO validate split
                    projectedNamespace = projected.Substring(0, split);
                    projectedTypeName = projected.Substring(split + 1);
                }
                else if (originalQualifiedName.StartsWith(platformPrefix))
                {
                    string partialName = originalQualifiedName.Substring(platformPrefix.Length);
                    int split = partialName.LastIndexOf('.');
                    projectedNamespace = "DinoCat.Wpf.Projected";
                    if (split != -1)
                        projectedNamespace += partialName.Substring(0, split);
                    projectedTypeName = partialName.Substring(split + 1);
                }
                else
                    return;

                // No support for collections/array parameters yet
                var constructor = symbol.InstanceConstructors.FirstOrDefault();
                if (constructor == null || constructor.Parameters.Any(p => p.IsParams))
                    return;

                StringBuilder source = new();
                source.AppendLine($@"#pragma warning disable CS0108
#pragma warning disable CS8604
namespace {projectedNamespace}
{{");

                // If there's a parameter named "content" assume it's the content attribute,
                // otherwise if there's only one non-default parameter pick that.
                // TODO Configure with attribute as well? Pick the first?? Could improve.
                var contentParameter = constructor.Parameters.FirstOrDefault(p => p.Name == "content");
                var requiredCount = constructor.Parameters.Count(p => !p.IsOptional);
                if (contentParameter != null || requiredCount == 1)
                {
                    string name;
                    if (contentParameter != null)
                        name = "Content";
                    else
                        name = Capitalize(constructor.Parameters.First(p => !p.IsOptional).Name);
                    source.AppendLine($"    [global::System.Windows.Markup.ContentProperty(\"{name}\")]");
                }

                source.Append(' ', 4);
                source.AppendLine($@"{symbol.DeclaredAccessibility.Format()} class {projectedTypeName} : global::DinoCat.Wpf.Host
    {{
        public {projectedTypeName}()
        {{
            RootElement = () => new {originalQualifiedName}(");
                // Transfer properties to constructor arguments
                for (int i = 0; i < constructor.Parameters.Length; ++i)
                {
                    var param = constructor.Parameters[i];
                    var (_, convertFormat) = MapParameter(param.Type);
                    var converted = string.Format(convertFormat, Capitalize(param.Name));
                    source.Append(' ', 16);
                    source.Append($"{param.Name}: {converted}");
                    if (i + 1 != constructor.Parameters.Length)
                        source.AppendLine(",");
                }
                source.AppendLine(");");
                source.AppendLine("        }");

                foreach (var param in constructor.Parameters)
                {
                    var name = Capitalize(param.Name);
                    source.Append(' ', 8);

                    if (types.Action.Equals(param.Type, SymbolEqualityComparer.Default))
                    {
                        source.AppendLine($"public event global::System.EventHandler<global::System.EventArgs>? {name};");
                        source.AppendLine();
                        continue;
                    }

                    var (wpfType, _) = MapParameter(param.Type);
                    // TODO: Transfer default value into PropertyMetadata
                    source.AppendLine(@$"public static readonly global::System.Windows.DependencyProperty {name}Property =
            global::System.Windows.DependencyProperty.Register(nameof({name}), typeof({wpfType}), typeof({projectedTypeName}),
                new global::System.Windows.PropertyMetadata(null, OnPropertyChanged));
        public {wpfType} {name}
        {{
            get => ({wpfType})GetValue({name}Property);
            set => SetValue({name}Property, value);
        }}
");
                }

                source.Append(' ', 4);
                source.AppendLine($@"private static void OnPropertyChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs args) =>
            (({projectedTypeName})sender).Refresh();
    }}
}}");

                var sourceFile = $"{projectedNamespace}.{projectedTypeName}.cs";
                context.AddSource(sourceFile, source.ToString());


                // I can't find a way to see the generator output. Just write it to a file for debugging.
                if (generatePlatformTypes)
                {
                    //Directory.CreateDirectory("C:\\DinoCat\\src\\DinoCat.Wpf\\DinoCat.Wpf.Generator\\DinoCat.Wpf.Generator.ToWpfGenerator");
                    //File.WriteAllText($"C:\\DinoCat\\src\\DinoCat.Wpf\\DinoCat.Wpf.Generator\\DinoCat.Wpf.Generator.ToWpfGenerator\\{sourceFile}.cs", source.ToString());
                }
                else
                {
                    //Directory.CreateDirectory("C:\\DinoCat\\examples\\Interop.Wpf\\DinoCat.Wpf.Generator\\DinoCat.Wpf.Generator.ToWpfGenerator");
                    //File.WriteAllText($"\\DinoCat\\examples\\Interop.Wpf\\DinoCat.Wpf.Generator\\DinoCat.Wpf.Generator.ToWpfGenerator\\{sourceFile}.cs", source.ToString());
                }
            }

            (string, string) MapParameter(ITypeSymbol symbol)
            {
                if (symbol is INamedTypeSymbol named && mappedTypes.TryGetValue(named, out var m))
                    return m;
                return (symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), "{0}");
            }

            static string Capitalize(string name)
            {
                var first = name.Substring(0, 1);
                return first.ToUpperInvariant() + name.Substring(1);
            }

            bool IsAccessible(Accessibility access) =>
                access == Accessibility.Public || (allowInternal && access == Accessibility.Internal);
        
        }
    }
}
