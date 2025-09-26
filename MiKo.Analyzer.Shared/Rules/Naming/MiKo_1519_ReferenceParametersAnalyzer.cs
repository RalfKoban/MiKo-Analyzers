using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1519_ReferenceParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1519";

        public MiKo_1519_ReferenceParametersAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;

            if (name.Contains("Reference", StringComparison.OrdinalIgnoreCase))
            {
                var betterName = FindBetterName(name);

                if (betterName != name)
                {
                    return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.Length > 9
                                                             ? name.AsCachedBuilder().Without("reference").Without("Reference").ToLowerCaseAt(0).ToStringAndRelease() // simply remove both as we need to check them anyway (so we save some calls)
                                                             : name;
    }
}