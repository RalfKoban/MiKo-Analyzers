using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1061_TryMethodOutParameterNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1061";

        public MiKo_1061_TryMethodOutParameterNameAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            if (symbol.IsTestClass()) return Enumerable.Empty<Diagnostic>(); // ignore tests

            List<Diagnostic> results = null;
            foreach (var finding in symbol.GetMembers().OfType<IMethodSymbol>().Select(AnalyzeTryMethod).Where(_ => _ != null))
            {
                if (results == null) results = new List<Diagnostic>();
                results.Add(finding);
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private Diagnostic AnalyzeTryMethod(IMethodSymbol method)
        {
            if (method.Name.StartsWith("Try", StringComparison.Ordinal))
            {
                const string ParameterName = "result";

                var outParameter = method.Parameters.FirstOrDefault(_ => _.RefKind == RefKind.Out);
                if (outParameter != null && outParameter.Name != ParameterName)
                    return Issue(outParameter, ParameterName);
            }

            return null;
        }
    }
}