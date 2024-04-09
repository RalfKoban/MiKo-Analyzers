using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1031_TypeModelSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1031";

        public MiKo_1031_TypeModelSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.Implements("Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder") is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeEntityMarkers(symbol);
    }
}