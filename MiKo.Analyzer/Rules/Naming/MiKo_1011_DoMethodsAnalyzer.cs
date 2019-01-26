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

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;
            var escapedMethod = methodName;

            var found = ContainsPhrase(methodName);
            if (found)
            {
                // special case "Does"
                if (ContainsPhrase(methodName, DoesPhrase))
                {
                    escapedMethod = EscapeValidPhrases(methodName.Remove(DoesPhrase));
                    found = !method.IsTestMethod(); // ignore tests
                }
                else
                {
                    escapedMethod = EscapeValidPhrases(methodName);
                    found = ContainsPhrase(escapedMethod);
                }
            }

            return found
                   ? new[] { ReportIssue(method, UnescapeValidPhrases(escapedMethod.RemoveAll(DoPhrase))) }
                   : Enumerable.Empty<Diagnostic>();
        }

        private static bool ContainsPhrase(string methodName, string phrase = DoPhrase) => methodName.Contains(phrase, StringComparison.Ordinal);

        private static string EscapeValidPhrases(string methodName)
        {
            var escapedMethod = methodName
                                .Replace("Doc", EscapedPhrase + "c")
                                .Replace("DoEvents", EscapedPhrase + "Events")
                                .Replace("Double", EscapedPhrase + "uble")
                                .Replace("Doubt", EscapedPhrase + "ubt")
                                .Replace("Domain", EscapedPhrase + "main")
                                .Replace("Done", EscapedPhrase + "ne")
                                .Replace("Dot", EscapedPhrase + "t")
                                .Replace("Down", EscapedPhrase + "wn");
            return escapedMethod;
        }

        private static string UnescapeValidPhrases(string methodName) => methodName.Replace(EscapedPhrase, DoPhrase);
    }
}