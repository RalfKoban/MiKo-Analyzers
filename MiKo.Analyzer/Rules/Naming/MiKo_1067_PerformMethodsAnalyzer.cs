using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1067_PerformMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1067";

        private const string Phrase = "Perform";

        public MiKo_1067_PerformMethodsAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol symbol)
        {
            var name = symbol.Name.Without(Phrase);
            return NamesFinder.TryMakeVerb(name, out var result) ? result : name;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;

            var found = ContainsPhrase(methodName) && ContainsPhrase(methodName.Without("Performance").Without("Performed"));

            return found
                       ? new[] { Issue(method) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool ContainsPhrase(string methodName) => methodName.Contains(Phrase, StringComparison.Ordinal);
    }
}