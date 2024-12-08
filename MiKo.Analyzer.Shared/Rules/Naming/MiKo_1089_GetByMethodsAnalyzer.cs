using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

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

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name.AsSpan();

            return symbolName.EndsWith("Repository", StringComparison.Ordinal) || Pluralizer.IsPlural(symbolName);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol)
                                                                   && symbol.IsExtensionMethod is false
                                                                   && symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.Length > Get.Length && symbolName.StartsWith(Get, StringComparison.Ordinal))
            {
                if (ShallAnalyze(symbol.ContainingType))
                {
                    return new[] { Issue(symbol, CreateBetterNameProposal(FindBetterName(symbolName))) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.AsCachedBuilder().Remove(0, Get.Length).ReplaceWithCheck("By", "With").ToStringAndRelease();
    }
}