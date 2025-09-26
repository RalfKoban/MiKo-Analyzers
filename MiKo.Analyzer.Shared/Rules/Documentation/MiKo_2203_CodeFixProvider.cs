using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2203_CodeFixProvider)), Shared]
    public sealed class MiKo_2203_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private const string ReplacementTerm = "unique identifier";

//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2203";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            return GetUpdatedSyntax(syntax, issue, ReplacementMap, Constants.Comments.Guids[0], ReplacementTerm);
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

                var replacement = term.ToUpperInvariant().Replace("GUID", ReplacementTerm);
                var replacementWithA = "An " + replacement.TrimStart();

                result[resultIndex++] = new Pair(termWithA, replacementWithA);
                result[resultIndex++] = new Pair(term, replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}