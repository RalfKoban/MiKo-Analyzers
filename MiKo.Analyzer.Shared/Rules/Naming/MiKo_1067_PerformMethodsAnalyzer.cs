using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

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

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (ContainsPhrase(methodName) && ContainsPhrase(methodName.AsBuilder().Without("Performance").Without("Performed").ToString()))
            {
                var proposal = FindBetterName(methodName);

                return new[] { Issue(symbol, CreateBetterNameProposal(proposal)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool ContainsPhrase(string methodName) => methodName.Contains(Phrase, StringComparison.Ordinal);

        private static string FindBetterName(string symbolName)
        {
            var name = symbolName.Without(Phrase);

            return Verbalizer.TryMakeVerb(name, out var result) ? result : name;
        }
    }
}