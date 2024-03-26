using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1012_FireMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1012";

        private static readonly string[] FirePhrases = { "Fire", "_fire", "Firing", "_firing" };
        private static readonly string[] FirewallPhrases = { "Firewall", "_firewall" };

        public MiKo_1012_FireMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            var forbidden = methodName.ContainsAny(FirePhrases) && methodName.ContainsAny(FirewallPhrases) is false;

            if (forbidden)
            {
                var proposal = FindBetterName(symbol);

                yield return Issue(symbol, proposal, CreateBetterNameProposal(proposal));
            }
        }

        private static string FindBetterName(IMethodSymbol method) => new StringBuilder(method.Name).ReplaceWithCheck("Fire", "Raise")
                                                                                                    .ReplaceWithCheck("_fire", "_raise")
                                                                                                    .ReplaceWithCheck("Firing", "Raising")
                                                                                                    .ReplaceWithCheck("_firing", "_raising")
                                                                                                    .ToString();
    }
}