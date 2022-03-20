using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static string FindBetterName(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;
            var escapedMethod = methodName;

            var found = ContainsPhrase(methodName);
            if (found)
            {
                // special case "Does"
                if (ContainsPhrase(methodName, DoesPhrase))
                {
                    escapedMethod = EscapeValidPhrases(methodName.Without(DoesPhrase));
                    found = symbol.IsTestMethod() is false; // ignore tests
                }
                else
                {
                    escapedMethod = EscapeValidPhrases(methodName);
                    found = ContainsPhrase(escapedMethod);
                }
            }

            if (found)
            {
                var proposal = UnescapeValidPhrases(escapedMethod.Without(DoPhrase));
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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsTestMethod() is false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => symbol.IsTestMethod()
                                                                                                                               ? Enumerable.Empty<Diagnostic>() // don't consider local functions inside tests
                                                                                                                               : base.AnalyzeLocalFunctions(symbol, compilation);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var proposal = FindBetterName(symbol);

            if (proposal != null)
            {
                yield return Issue(symbol, proposal);
            }
        }

        private static bool ContainsPhrase(string methodName, string phrase = DoPhrase) => methodName.Contains(phrase, StringComparison.Ordinal);

        private static string EscapeValidPhrases(string methodName)
        {
            var escapedMethod = methodName
                                .Replace("Doc", EscapedPhrase + "c")
                                .Replace("Dog", EscapedPhrase + "g")
                                .Replace("Dot", EscapedPhrase + "t")
                                .Replace("Done", EscapedPhrase + "ne")
                                .Replace("DoEvents", EscapedPhrase + "Events")
                                .Replace("Domain", EscapedPhrase + "main")
                                .Replace("Double", EscapedPhrase + "uble")
                                .Replace("Doubt", EscapedPhrase + "ubt")
                                .Replace("Down", EscapedPhrase + "wn");

            return escapedMethod;
        }

        private static string UnescapeValidPhrases(string methodName) => methodName.Replace(EscapedPhrase, DoPhrase);
    }
}