using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2202_CodeFixProvider)), Shared]
    public sealed class MiKo_2202_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private const string ReplacementTerm = "identifier";

//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2202";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            return GetUpdatedSyntax(syntax, issue, ReplacementMap, Constants.Comments.IdTerm, ReplacementTerm);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.IdTermWithDelimiters;

            var result = new Pair[6 * terms.Length];
            var resultIndex = 0;

            foreach (var term in terms)
            {
                var alternative = term.Replace('i', 'I');

                var termWithA = "A " + term.TrimStart();
                var alternativeWithA = "A " + alternative.TrimStart();

                var replacement = term.Replace(Constants.Comments.IdTerm, ReplacementTerm);
                var replacementWithA = "An " + replacement.TrimStart();

                result[resultIndex++] = new Pair(termWithA, replacementWithA);
                result[resultIndex++] = new Pair(term, replacement);

                result[resultIndex++] = new Pair(alternativeWithA, replacementWithA);
                result[resultIndex++] = new Pair(alternative, replacement);

                // also consider the upper case as its a commonly used abbreviation
                result[resultIndex++] = new Pair(alternativeWithA.ToUpperInvariant(), replacementWithA);
                result[resultIndex++] = new Pair(alternative.ToUpperInvariant(), replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}