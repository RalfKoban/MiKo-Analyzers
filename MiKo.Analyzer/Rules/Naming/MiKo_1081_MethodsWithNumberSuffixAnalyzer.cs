using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1081_MethodsWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1081";

        public MiKo_1081_MethodsWithNumberSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name.EndsWithNumber() && symbol.Name.EndsWithAny(Constants.Markers.OSBitNumbers) is false
                                                                                            ? new[] { Issue(symbol) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}