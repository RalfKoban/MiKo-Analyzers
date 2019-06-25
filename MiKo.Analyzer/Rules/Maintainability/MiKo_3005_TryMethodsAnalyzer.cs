using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3005_TryMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3005";

        public MiKo_3005_TryMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsTestClass() is false;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol) => symbol.GetMembers().OfType<IMethodSymbol>().Select(AnalyzeTryMethod).Where(_ => _ != null);

        private Diagnostic AnalyzeTryMethod(IMethodSymbol method)
        {
            if (method.IsOverride) return null;
            if (!method.Name.StartsWith("Try", StringComparison.Ordinal)) return null;
            if (method.ReturnType.SpecialType == SpecialType.System_Boolean && (method.Parameters.Any() && method.Parameters.Last().RefKind == RefKind.Out)) return null;

            return Issue(method);
        }
    }
}