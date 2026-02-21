using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2041_CodeFixProvider)), Shared]
    public sealed class MiKo_2041_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2041";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax)
        {
            var syntaxNodes = syntax.GetSummaryXmls(Constants.Comments.InvalidSummaryCrefXmlTags).ToList();

            var updatedSyntax = syntax.Without(syntaxNodes);

            // identify the now-empty summaries and remove those
            var emptySummaries = updatedSyntax.GetSummaryXmls().Where(_ => _.GetTextTrimmed().IsNullOrEmpty()).ToList();

            if (emptySummaries.Count is 0)
            {
                return updatedSyntax.AddContent(syntaxNodes.ToArray(_ => _.WithLeadingXmlCommentExterior().WithEndOfLine()));
            }

            return updatedSyntax.ReplaceNodes(emptySummaries, _ => syntaxNodes);
        }
    }
}