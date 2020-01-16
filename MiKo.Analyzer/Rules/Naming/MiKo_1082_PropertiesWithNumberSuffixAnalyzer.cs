using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1082_PropertiesWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1082";

        public MiKo_1082_PropertiesWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol)
        {
            if (symbol.Name.EndsWithNumber() && symbol.GetReturnType()?.Name.EndsWithNumber() is true)
            {
                yield return Issue(symbol);
            }
        }
    }
}