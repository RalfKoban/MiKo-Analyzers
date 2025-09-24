using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2222_CodeFixProvider)), Shared]
    public sealed class MiKo_2222_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private const string ReplacementTerm = "identification";

//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2222";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            return GetUpdatedSyntax(syntax, issue, ReplacementMap, Constants.Comments.IdentTerm, ReplacementTerm);
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var terms = Constants.Comments.IdentTermWithDelimiters;

            var result = new Pair[3 * terms.Length];
            var resultIndex = 0;

            foreach (var term in terms)
            {
                var replacement = term.Replace(Constants.Comments.IdentTerm, ReplacementTerm);
                result[resultIndex++] = new Pair(term, replacement);

                // alternative 1
                result[resultIndex++] = new Pair(term.Replace('i', 'I'), replacement);

                // alternative 2
                result[resultIndex++] = new Pair(term.ToUpperCase(), replacement);
            }

            return result;
        }

//// ncrunch: rdi default
    }
}