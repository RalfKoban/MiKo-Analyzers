using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1525_NonBooleanPropertiesPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1525";

        private static readonly string[] Prefixes = { "Is", "Are", "Can", "Has", "Contains" };

        public MiKo_1525_NonBooleanPropertiesPrefixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            if (symbol.GetReturnType().IsBoolean())
            {
                return Array.Empty<Diagnostic>();
            }

            var symbolName = symbol.Name.AsSpan();

            foreach (var prefix in Prefixes)
            {
                if (symbolName.Length > prefix.Length && symbolName[prefix.Length].IsUpperCase() && symbolName.StartsWith(prefix.AsSpan()))
                {
                    return new[] { Issue(symbol, prefix) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}