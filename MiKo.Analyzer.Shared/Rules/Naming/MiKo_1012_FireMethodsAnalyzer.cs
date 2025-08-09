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

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.ContainsAny(FirePhrases, StringComparison.OrdinalIgnoreCase) && methodName.ContainsAny(FirewallPhrases, StringComparison.OrdinalIgnoreCase) is false)
            {
                var proposal = FindBetterName(symbol);

                return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(IMethodSymbol method) => method.Name
                                                                            .AsCachedBuilder()
                                                                            .ReplaceWithProbe("Fire", "Raise")
                                                                            .ReplaceWithProbe("_fire", "_raise")
                                                                            .ReplaceWithProbe("Firing", "Raising")
                                                                            .ReplaceWithProbe("_firing", "_raising")
                                                                            .ToStringAndRelease();
    }
}