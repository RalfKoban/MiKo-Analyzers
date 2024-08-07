using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2222_CodeFixProvider)), Shared]
    public sealed class MiKo_2222_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2222";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.IdentTerms, ReplacementMap);
        }

//// ncrunch: rdi off

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in Constants.Comments.IdentTerms)
            {
                var replacement = term.Replace(Constants.Comments.IdentTerm, "identification");
                dictionary.Add(term, replacement);

                // alternative 1
                dictionary.Add(term.Replace('i', 'I'), replacement);

                // alternative 2
                dictionary.Add(term.ToUpper(), replacement);
            }

            return dictionary;
        }

//// ncrunch: rdi default
    }
}