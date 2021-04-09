using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.StandardUI.Wpf.Generator
{
    static class ContextExtensions
    {
        public static void PrintDebug(this GeneratorExecutionContext context, string text)
        {
            context.ReportDiagnostic(Diagnostic.Create("DINO999", "StandardUI", text, DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 1));
        }
    }
}
