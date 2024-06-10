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

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWithCommonNumber())
            {
                return new[] { Issue(symbol, CreateBetterNameProposal(symbolName.WithoutNumberSuffix())) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}