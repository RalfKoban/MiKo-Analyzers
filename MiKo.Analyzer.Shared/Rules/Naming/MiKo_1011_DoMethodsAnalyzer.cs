using System;
using System.Collections.Generic;
using System.Linq;
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
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
        }

        private static string FindBetterName(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;
            var escapedMethod = methodName.AsBuilder();

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

                var proposal = escapedMethod.ToString();

                switch (proposal)
                {
                    case "": // special case 'Do'
                    case "Can": // special case 'CanDo'
                        return proposal + "Execute";

                    default:
                        return proposal;
                }
            }

            return null;
        }

        private static bool ContainsPhrase(string methodName, string phrase = DoPhrase) => methodName.Contains(phrase, StringComparison.Ordinal);

        private static void EscapeValidPhrases(StringBuilder methodName)
        {
            methodName.ReplaceWithCheck("Doc", EscapedPhrase + "c")
                      .ReplaceWithCheck("Dog", EscapedPhrase + "g")
                      .ReplaceWithCheck("Dot", EscapedPhrase + "t")
                      .ReplaceWithCheck("Done", EscapedPhrase + "ne")
                      .ReplaceWithCheck("Dont", EscapedPhrase + "nt")
                      .ReplaceWithCheck("DoEvents", EscapedPhrase + "Events")
                      .ReplaceWithCheck("Domain", EscapedPhrase + "main")
                      .ReplaceWithCheck("Double", EscapedPhrase + "uble")
                      .ReplaceWithCheck("Doubt", EscapedPhrase + "ubt")
                      .ReplaceWithCheck("Down", EscapedPhrase + "wn");
        }

        private static void UnescapeValidPhrases(StringBuilder methodName) => methodName.ReplaceWithCheck(EscapedPhrase, DoPhrase);
    }
}