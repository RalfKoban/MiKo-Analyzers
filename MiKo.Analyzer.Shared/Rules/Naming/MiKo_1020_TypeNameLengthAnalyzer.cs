using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1020_TypeNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1020";

        public MiKo_1020_TypeNameLengthAnalyzer() : base(Id, SymbolKind.NamedType, Constants.MaxNamingLengths.Types)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsTestClass() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => Analyze(symbol);
    }
}