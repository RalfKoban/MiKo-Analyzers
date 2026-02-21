using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2014_CodeFixProvider)), Shared]
    public sealed class MiKo_2014_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2014";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax)
        {
            var method = syntax.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), Constants.Comments.DisposeSummaryPhrase).WithTrailingXmlComment();

            var parameters = method.ParameterList.Parameters;

            if (parameters.Count is 0)
            {
                return SyntaxFactory.DocumentationComment(summary.WithEndOfLine());
            }

            var param = ParameterComment(parameters[0], Constants.Comments.DisposeParameterPhrase).WithEndOfLine();

            return SyntaxFactory.DocumentationComment(summary, param);
        }
    }
}