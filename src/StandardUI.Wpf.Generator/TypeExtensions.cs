using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.StandardUI.Wpf.Generator
{
    static class TypeExtensions
    {
        private static ConditionalWeakTable<INamedTypeSymbol, string> dinoType = new();

        public static string? GetDinoType(this ITypeSymbol me)
        {
            string? typeName;
            if (me is INamedTypeSymbol named)
                dinoType.TryGetValue(named, out typeName);
            else
                typeName = null;
            return typeName;
        }
        public static INamedTypeSymbol SetDinoType(this INamedTypeSymbol me, string typeName)
        {
            if (!dinoType.TryGetValue(me, out var _))
                dinoType.Add(me, typeName);
            return me;
        }

        public static bool IsSubclassOf(this INamedTypeSymbol me, INamedTypeSymbol super)
        {
            var current = me.BaseType;
            while (current != null)
            {
                if (current.Equals(super, SymbolEqualityComparer.Default))
                    return true;
                current = current.BaseType;
            }

            return false;
        }

        // Only works for private/protected/friend/internal/public for now
        public static string Format(this Accessibility accessibility) => accessibility.ToString().ToLower();
    }
}
