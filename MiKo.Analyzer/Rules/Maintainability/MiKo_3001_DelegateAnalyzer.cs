using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3001_DelegateAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3001";

        public MiKo_3001_DelegateAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Delegate
                                                                                               ? new[] { Issue(symbol) }
                                                                                               : Enumerable.Empty<Diagnostic>();
    }
}