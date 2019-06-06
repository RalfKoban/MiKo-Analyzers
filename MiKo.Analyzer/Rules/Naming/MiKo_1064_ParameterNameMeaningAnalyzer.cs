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
            var type = symbol.Type;
            if (string.Equals(symbol.Name, type.Name, StringComparison.OrdinalIgnoreCase))
            {
                var name = GetNameWithoutInterfacePrefix(type);

                if (name.Length > 1 && name.Substring(1).Any(_ => _.IsUpperCase()))
                {
                    return new[] { Issue(symbol) };
                }
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