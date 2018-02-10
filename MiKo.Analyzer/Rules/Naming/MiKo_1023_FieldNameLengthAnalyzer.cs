using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1023_FieldNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1023";

        public MiKo_1023_FieldNameLengthAnalyzer() : base(Id, SymbolKind.Field, 10)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => Analyze(symbol);
    }
}