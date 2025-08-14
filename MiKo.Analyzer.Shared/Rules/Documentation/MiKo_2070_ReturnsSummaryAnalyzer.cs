using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2070";

        public MiKo_2070_ReturnsSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol)
            {
                case IPropertySymbol _:
                    return true;

                case IMethodSymbol method:
                {
                    switch (method.Name)
                    {
                        case nameof(ToString):
                        case nameof(IEnumerable.GetEnumerator):
                            return false;

                        default:
                            return base.ShallAnalyze(method);
                    }
                }

                default:
                    return false;
            }
        }

        protected override bool ConsiderEmptyTextAsIssue(ISymbol symbol) => false;

        protected override Diagnostic NonTextStartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, GetProposal(symbol));

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.OrdinalIgnoreCase;

            var firstWord = valueText.Without(Constants.Comments.AsynchronouslyStartingPhrase) // skip over async starting phrase
                                     .FirstWord();

            if (firstWord.EqualsAny(Constants.Comments.ReturnWords, StringComparison.OrdinalIgnoreCase))
            {
                problematicText = valueText.FirstWord();

                return true;
            }

            return false;
        }

        private static string GetProposal(ISymbol symbol)
        {
            if (symbol is IMethodSymbol m)
            {
                var startText = m.ReturnType.IsBoolean()
                                ? Constants.Comments.DeterminesWhetherPhrase
                                : "Gets";

                if (m.IsAsync)
                {
                    return Constants.Comments.AsynchronouslyStartingPhrase + startText.ToLowerCaseAt(0);
                }

                return startText;
            }

            return "Gets";
        }
    }
}