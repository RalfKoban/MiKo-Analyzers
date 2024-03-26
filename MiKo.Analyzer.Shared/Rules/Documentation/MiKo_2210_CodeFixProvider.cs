using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2210_CodeFixProvider)), Shared]
    public sealed class MiKo_2210_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => "MiKo_2210";

        protected override string Title => Resources.MiKo_2210_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, Constants.Comments.InfoTerms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var term in Constants.Comments.InfoTerms)
            {
                var replacement = term.Replace(Constants.Comments.InfoTerm, "information");
                dictionary.Add(term, replacement);

                var alternative = term.Replace('i', 'I');
                dictionary.Add(alternative, replacement);
            }

            return dictionary;
        }
    }
}