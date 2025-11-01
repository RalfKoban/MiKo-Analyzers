using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2203_CodeFixProvider)), Shared]
    public sealed class MiKo_2203_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private const string SingleReplacementTerm = "unique identifier";
        private const string PluralReplacementTerm = "unique identifiers";

//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2203";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            var endingTerm = Constants.Comments.Guids[0];
            var endingReplacement = SingleReplacementTerm;

            var text = issue.Location.GetText();

            if (text.EndsWith("S", StringComparison.OrdinalIgnoreCase))
            {
                endingTerm = Constants.Comments.Guids[0] + "s";
                endingReplacement = PluralReplacementTerm;
            }

            return GetUpdatedSyntax(syntax, issue, ReplacementMap, endingTerm, endingReplacement);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.GuidTermsWithDelimiters;
            var result = new Pair[2 * terms.Length];
            var resultIndex = 0;

            foreach (var term in terms)
            {
                var termWithA = "A " + term.TrimStart();

                var upperTerm = term.ToUpperInvariant();

                var replacement = upperTerm.Contains("S", StringComparison.OrdinalIgnoreCase)
                                  ? upperTerm.Replace("GUIDS", PluralReplacementTerm)
                                  : upperTerm.Replace("GUID", SingleReplacementTerm);

                var replacementWithA = "An " + replacement.TrimStart();

                result[resultIndex++] = new Pair(termWithA, replacementWithA);
                result[resultIndex++] = new Pair(term, replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}