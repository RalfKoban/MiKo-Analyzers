using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_3215_PredicateUsageAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3001_DelegateAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3001";

        public MiKo_3001_DelegateAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Delegate;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation) => new[] { Issue(symbol) };
    }
}