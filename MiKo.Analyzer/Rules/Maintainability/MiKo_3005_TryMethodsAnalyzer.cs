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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (symbol.IsTestClass()) return Enumerable.Empty<Diagnostic>(); // ignore tests

            List<Diagnostic> results = null;
            foreach (var finding in symbol.GetMembers().OfType<IMethodSymbol>().Select(Analyze).Where(_ => _ != null))
            {
                if (results == null) results = new List<Diagnostic>();
                results.Add(finding);
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private Diagnostic Analyze(IMethodSymbol method)
        {
            if (method.IsOverride) return null;
            if (!method.Name.StartsWith("Try", StringComparison.Ordinal)) return null;
            if (method.ReturnType.SpecialType == SpecialType.System_Boolean && (method.Parameters.Any() && method.Parameters.Last().RefKind == RefKind.Out)) return null;

            return ReportIssue(method);
        }
    }
}