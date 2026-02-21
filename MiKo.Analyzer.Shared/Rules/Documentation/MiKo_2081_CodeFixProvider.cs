using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2081_CodeFixProvider)), Shared]
    public sealed class MiKo_2081_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2081";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax comment)
        {
            return CommentWithContent(comment, comment.WithoutText(Constants.Comments.FieldIsReadOnly).Add(XmlText(Constants.Comments.FieldIsReadOnly))); // place on new line
        }
    }
}