using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1011_DoMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1011";

        private const string DoPhrase = "Do";
        private const string DoesPhrase = "Does";
        private const string EscapedPhrase = "##";

        public MiKo_1011_DoMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => symbol.IsTestMethod() is false; // do not consider local functions inside tests

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var proposal = FindBetterName(symbol);

            if (proposal is null)
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
        }

        private static string FindBetterName(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;
            var escapedMethod = methodName.AsCachedBuilder();

            var found = ContainsPhrase(methodName);

            if (found)
            {
                // special case "Does"
                if (ContainsPhrase(methodName, DoesPhrase))
                {
                    EscapeValidPhrases(escapedMethod.Without(DoesPhrase));

                    found = symbol.IsTestMethod() is false; // ignore tests
                }
                else
                {
                    EscapeValidPhrases(escapedMethod);

                    found = ContainsPhrase(escapedMethod.ToString());
                }
            }

            if (found)
            {
                UnescapeValidPhrases(escapedMethod.Without(DoPhrase));

                var proposal = escapedMethod.ToStringAndRelease();

                switch (proposal)
                {
                    case "": // special case 'Do'
                    case "Can": // special case 'CanDo'
                        return proposal + "Execute";

                    default:
                        return proposal;
                }
            }
            else
            {
                StringBuilderCache.Release(escapedMethod);
            }

            return null;
        }

        private static bool ContainsPhrase(string methodName, string phrase = DoPhrase) => methodName.Contains(phrase, StringComparison.Ordinal);

        private static void EscapeValidPhrases(StringBuilder methodName)
        {
            methodName.ReplaceWithProbe("Doc", EscapedPhrase + "c")
                      .ReplaceWithProbe("Dog", EscapedPhrase + "g")
                      .ReplaceWithProbe("Dot", EscapedPhrase + "t")
                      .ReplaceWithProbe("Done", EscapedPhrase + "ne")
                      .ReplaceWithProbe("Dont", EscapedPhrase + "nt")
                      .ReplaceWithProbe("DoEvents", EscapedPhrase + "Events")
                      .ReplaceWithProbe("Domain", EscapedPhrase + "main")
                      .ReplaceWithProbe("Double", EscapedPhrase + "uble")
                      .ReplaceWithProbe("Doubt", EscapedPhrase + "ubt")
                      .ReplaceWithProbe("Down", EscapedPhrase + "wn");
        }

        private static void UnescapeValidPhrases(StringBuilder methodName) => methodName.ReplaceWithProbe(EscapedPhrase, DoPhrase);
    }
}