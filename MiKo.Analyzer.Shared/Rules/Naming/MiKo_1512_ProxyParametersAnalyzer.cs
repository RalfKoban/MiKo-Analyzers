using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1512_ProxyParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1512";

        public MiKo_1512_ProxyParametersAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;

            if (name.Contains("Proxy", StringComparison.OrdinalIgnoreCase))
            {
                var betterName = FindBetterName(name);

                if (betterName != name)
                {
                    return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => name.Length > 5
                                                             ? name.Without("proxy").Without("Proxy").ToLowerCaseAt(0) // simply remove both as we need to check them anyway (so we save some calls)
                                                             : name;
    }
}