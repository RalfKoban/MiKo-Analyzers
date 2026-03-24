using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1524_MethodPrefixedWithSubAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1524";

        public MiKo_1524_MethodPrefixedWithSubAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            const string Prefix = "Sub_";

            var symbolName = symbol.Name;

            if (symbolName.Length > Prefix.Length && symbolName.StartsWith(Prefix, StringComparison.Ordinal))
            {
                var betterName = symbolName.Substring(Prefix.Length);

                return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}