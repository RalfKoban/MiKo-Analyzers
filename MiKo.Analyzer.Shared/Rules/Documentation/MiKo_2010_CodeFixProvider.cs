using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2010_CodeFixProvider)), Shared]
    public sealed class MiKo_2010_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2010";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            const string Text = Constants.Comments.SealedClassPhrase;

            return CommentWithContent(comment, comment.WithoutText(Text).Add(XmlText(Text)));
        }
    }
}