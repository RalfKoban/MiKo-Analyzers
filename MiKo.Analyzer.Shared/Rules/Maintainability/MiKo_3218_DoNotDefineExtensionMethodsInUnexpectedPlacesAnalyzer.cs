using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3218_DoNotDefineExtensionMethodsInUnexpectedPlacesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3218";

        public MiKo_3218_DoNotDefineExtensionMethodsInUnexpectedPlacesAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsExtensionMethod;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => symbol.ContainingType.Name.Contains("Extension")
                                                                                                             ? Enumerable.Empty<Diagnostic>()
                                                                                                             : new[] { Issue(symbol) };
    }
}