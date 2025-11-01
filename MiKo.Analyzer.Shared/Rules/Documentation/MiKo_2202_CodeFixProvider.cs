using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2202_CodeFixProvider)), Shared]
    public sealed class MiKo_2202_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private const string SingleReplacementTerm = "identifier";
        private const string PluralReplacementTerm = "identifiers";

//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2202";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            var endingTerm = Constants.Comments.IdTerm;
            var endingReplacement = SingleReplacementTerm;

            var text = issue.Location.GetText();

            if (IsPlural(text))
            {
                endingTerm = Constants.Comments.IdsTerm;
                endingReplacement = PluralReplacementTerm;
            }

            return GetUpdatedSyntax(syntax, issue, ReplacementMap, endingTerm, endingReplacement);
        }

//// ncrunch: rdi off

        private static bool IsPlural(string term) => term.EndsWith("S", StringComparison.OrdinalIgnoreCase);

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.IdTermsWithDelimiters;

            var result = new List<Pair>(2 * 8 * terms.Length);

            foreach (var value in Constants.Comments.IdTerms)
            {
                foreach (var term in terms)
                {
                    var alternative = term.Replace('i', 'I');

                    var termWithA = "A " + term.TrimStart();
                    var alternativeWithA = "A " + alternative.TrimStart();

                    var isPlural = IsPlural(value);

                    var replacement = term.Replace(value, isPlural ? PluralReplacementTerm : SingleReplacementTerm);
                    var replacementWithA = "An " + replacement.TrimStart();

                    result.Add(new Pair(termWithA, replacementWithA));
                    result.Add(new Pair(term, replacement));

                    result.Add(new Pair(alternativeWithA, replacementWithA));
                    result.Add(new Pair(alternative, replacement));

                    // also consider the upper case as it's a commonly used abbreviation
                    var upperAlternativeWithA = alternativeWithA.ToUpperInvariant();
                    var upperAlternative = alternative.ToUpperInvariant();

                    result.Add(new Pair(upperAlternativeWithA, replacementWithA));
                    result.Add(new Pair(upperAlternative, replacement));

                    if (isPlural)
                    {
                        result.Add(new Pair(upperAlternativeWithA.Replace('S', 's'), replacementWithA));
                        result.Add(new Pair(upperAlternative.Replace('S', 's'), replacement));
                    }
                }
            }

            return result.Distinct().ToArray();
        }

//// ncrunch: rdi default
    }
}