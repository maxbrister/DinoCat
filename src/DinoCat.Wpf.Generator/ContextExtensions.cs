using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCat.Wpf.Generator
{
    static class ContextExtensions
    {
        public static void PrintDebug(this GeneratorExecutionContext context, string text)
        {
            context.ReportDiagnostic(Diagnostic.Create("DINO999", "DinoCat", text, DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 1));
        }
    }
}
