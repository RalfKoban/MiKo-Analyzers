using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2081_CodeFixProvider)), Shared]
    public sealed class MiKo_2081_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2081";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            return CommentWithContent(comment, comment.WithoutText(Constants.Comments.FieldIsReadOnly).Add(XmlText(Constants.Comments.FieldIsReadOnly))); // place on new line
        }
    }
}