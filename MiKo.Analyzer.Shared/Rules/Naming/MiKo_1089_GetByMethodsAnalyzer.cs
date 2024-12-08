using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1089_GetByMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1089";

        private const string Get = nameof(Get);

        public MiKo_1089_GetByMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            return symbolName.StartsWith(Get, StringComparison.Ordinal)
                       ? new[] { Issue(symbol, CreateBetterNameProposal(FindBetterName(symbolName))) }
                       : Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.AsCachedBuilder().Remove(0, Get.Length).ReplaceWithCheck("By", "With").ToStringAndRelease();
    }
}