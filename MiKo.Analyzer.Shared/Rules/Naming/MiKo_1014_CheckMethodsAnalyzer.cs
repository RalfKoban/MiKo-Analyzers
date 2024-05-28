using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1014_CheckMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1014";

        private const string Phrase = "Check";
        private const string HasPhrase = "CheckFor";
        private const string SpecialHasPhrase = "CheckFormat";

        private static readonly string[] StartingPhrases = { "CheckIn", "CheckOut", "CheckAccess" };

        public MiKo_1014_CheckMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => symbol.IsTestMethod() is false; // do not consider local functions inside tests

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;
            var forbidden = methodName.StartsWith(Phrase, StringComparison.Ordinal) && methodName.StartsWithAny(StartingPhrases, StringComparison.Ordinal) is false;

            if (forbidden)
            {
                var betterName = FindBetterName(symbol);

                yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
            }
        }

        private static string FindBetterName(IMethodSymbol method)
        {
            var prefix = FindBetterPrefix(method);

            var phrase = prefix == "Has" && method.Name != SpecialHasPhrase
                         ? HasPhrase
                         : Phrase;

            return prefix + method.Name.Substring(phrase.Length);
        }

        private static string FindBetterPrefix(IMethodSymbol method)
        {
            if (method.ReturnsVoid)
            {
                return method.Parameters.Any()
                       ? "Validate"
                       : "Verify";
            }

            if (method.ReturnType.IsBoolean())
            {
                var isHasCandidate = method.Name.StartsWith(HasPhrase, StringComparison.Ordinal);

                return isHasCandidate
                       ? "Has"
                       : "Can";
            }

            return "Find";
        }
    }
}