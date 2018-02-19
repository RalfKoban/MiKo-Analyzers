using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1011_DoMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1011";

        private const string DoPhrase = "Do";
        private const string EscapedPhrase = "##";

        public MiKo_1011_DoMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsInterfaceImplementationOf<ICommand>()) return Enumerable.Empty<Diagnostic>();

            var methodName = method.Name;

            const StringComparison Comparison = StringComparison.Ordinal;
            if (methodName.IndexOf(DoPhrase, Comparison) == -1) return Enumerable.Empty<Diagnostic>();

            var escapedMethod = EscapeValidPhrases(methodName);
            if (escapedMethod.IndexOf(DoPhrase, Comparison) == -1) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(method, UnescapeValidPhrases(escapedMethod.Replace(DoPhrase, string.Empty))) };
        }

        private static string EscapeValidPhrases(string methodName)
        {
            var escapedMethod = methodName
                                .Replace("Doc", EscapedPhrase + "c")
                                .Replace("Does", EscapedPhrase + "es")
                                .Replace("DoEvents", EscapedPhrase + "Events")
                                .Replace("Double", EscapedPhrase + "uble")
                                .Replace("Domain", EscapedPhrase + "main")
                                .Replace("Done", EscapedPhrase + "ne")
                                .Replace("Dot", EscapedPhrase + "t")
                                .Replace("Down", EscapedPhrase + "wn");
            return escapedMethod;
        }

        private static string UnescapeValidPhrases(string methodName) => methodName.Replace(EscapedPhrase, DoPhrase);
    }
}