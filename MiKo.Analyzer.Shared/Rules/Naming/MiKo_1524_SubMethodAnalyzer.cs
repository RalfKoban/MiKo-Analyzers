using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1524_SubMethodAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1524";

        public MiKo_1524_SubMethodAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.StartsWith("Sub_", StringComparison.Ordinal))
            {
                var betterName = CreateBetterNameProposal(symbolName.Substring(4));

                return new[] { Issue(symbol, betterName) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}