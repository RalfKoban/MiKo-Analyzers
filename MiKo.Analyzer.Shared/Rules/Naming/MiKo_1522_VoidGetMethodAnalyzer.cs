using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1522_VoidGetMethodAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1522";

        public MiKo_1522_VoidGetMethodAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.ReturnsVoid)
            {
                if (symbol.Name.StartsWith("Get", StringComparison.Ordinal))
                {
                    return new[] { Issue(symbol) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}