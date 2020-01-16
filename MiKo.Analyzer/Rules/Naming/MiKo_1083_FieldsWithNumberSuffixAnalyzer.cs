using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1083_FieldsWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1083";

        public MiKo_1083_FieldsWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            if (symbol.Name.EndsWithNumber() && symbol.Type.Name.EndsWithNumber())
            {
                yield return Issue(symbol);
            }
        }
    }
}