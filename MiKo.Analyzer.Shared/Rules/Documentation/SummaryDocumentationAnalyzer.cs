using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected SummaryDocumentationAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var summaryXmls = comment.GetSummaryXmls();

            if (summaryXmls.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = symbol.GetDocumentationCommentXml();

            var summaries = CommentExtensions.GetSummaries(commentXml);

            return AnalyzeSummaries(comment, symbol, summaryXmls, commentXml, summaries);
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, string commentXml, IReadOnlyCollection<string> summaries) => Array.Empty<Diagnostic>();
    }
}