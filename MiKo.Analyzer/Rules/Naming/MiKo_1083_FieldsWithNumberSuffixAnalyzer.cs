using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.Name.EndsWithNumber();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => symbol.Name.EndsWithNumber() && symbol.Name.EndsWithAny(Constants.Markers.OSBitNumbers) is false
                                                                                           ? new[] { Issue(symbol) }
                                                                                           : Enumerable.Empty<Diagnostic>();
    }
}