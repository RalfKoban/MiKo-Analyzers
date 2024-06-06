using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3211_PublicTypesShouldNotHaveFinalizerAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3211";

        public MiKo_3211_PublicTypesShouldNotHaveFinalizerAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Destructor;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => symbol.ContainingType.DeclaredAccessibility == Accessibility.Public
                                                                                                             ? new[] { Issue(symbol) }
                                                                                                             : Enumerable.Empty<Diagnostic>();
    }
}