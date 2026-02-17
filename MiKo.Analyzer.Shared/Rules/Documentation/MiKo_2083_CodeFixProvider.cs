using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2083_CodeFixProvider)), Shared]
    public sealed class MiKo_2083_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2083";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            return CommentWithContent(comment, comment.WithoutText(Constants.Comments.FieldIsReadOnly));
        }
    }
}