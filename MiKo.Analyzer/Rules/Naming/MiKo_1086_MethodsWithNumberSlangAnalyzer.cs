using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1086_MethodsWithNumberSlangAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1086";

        private static readonly char[] SlangNumbers = { '2', '4' };

        public MiKo_1086_MethodsWithNumberSlangAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var symbolName = symbol.Name;

            var index = symbolName.IndexOfAny(SlangNumbers);

            // ignore first character as that is never a number
            if (index > 0 && index < (symbolName.Length - 1) && symbolName[index + 1].IsLetter())
            {
                yield return Issue(symbol);
            }
        }
    }
}