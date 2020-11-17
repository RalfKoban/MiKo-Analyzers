using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1023_FieldNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1023";

        public MiKo_1023_FieldNameLengthAnalyzer() : base(Id, SymbolKind.Field, Constants.MaxNamingLengths.Fields)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsConst is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => Analyze(symbol);
    }
}