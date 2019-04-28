using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1060_MethodsWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1060";

        public MiKo_1060_MethodsWithNumberSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name.Last().IsNumber()
                                                                                            ? new[] { Issue(symbol) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}