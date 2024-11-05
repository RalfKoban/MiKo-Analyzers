using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2041_CodeFixProvider)), Shared]
    public sealed class MiKo_2041_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2041";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var syntaxNodes = syntax.GetSummaryXmls(Constants.Comments.InvalidSummaryCrefXmlTags).ToList();

            var updatedSyntax = syntax.Without(syntaxNodes);

            // identify the now-empty summaries and remove those
            var emptySummaries = updatedSyntax.GetSummaryXmls().Where(_ => _.GetTextTrimmed().IsNullOrEmpty()).ToList();

            if (emptySummaries.Count == 0)
            {
                return updatedSyntax.AddContent(syntaxNodes.ToArray(_ => _.WithLeadingXmlCommentExterior().WithEndOfLine()));
            }

            return updatedSyntax.ReplaceNodes(emptySummaries, _ => syntaxNodes);
        }
    }
}