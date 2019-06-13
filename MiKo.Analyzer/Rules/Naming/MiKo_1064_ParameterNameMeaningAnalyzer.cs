using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1064_ParameterNameMeaningAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1064";

        public MiKo_1064_ParameterNameMeaningAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol)
        {
            var symbolName = symbol.Name;
            var typeName = GetNameWithoutInterfacePrefix(symbol.Type);

            if (!string.Equals(symbolName, typeName, StringComparison.OrdinalIgnoreCase))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbol.ContainingSymbol is IMethodSymbol method)
            {
                if (!ShallAnalyze(method))
                {
                    // ignore overrides/interfaces as the signatures should match the base signature
                    return Enumerable.Empty<Diagnostic>();
                }

                if (method.MethodKind == MethodKind.Constructor && symbol.ContainingType.GetMembers().OfType<IPropertySymbol>().Any(_ => string.Equals(symbolName, _.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    // ignore those ctor parameters that get assigned to a property having the same name
                    return Enumerable.Empty<Diagnostic>();
                }
            }

            if (typeName.Length > 1 && typeName.Substring(1).Any(_ => _.IsUpperCase()))
            {
                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static string GetNameWithoutInterfacePrefix(ITypeSymbol type)
        {
            var typeName = type.Name;

            return type.TypeKind == TypeKind.Interface && typeName.StartsWith("I", StringComparison.Ordinal) && typeName.Length > 1
                       ? typeName.Substring(1)
                       : typeName;
        }
    }
}