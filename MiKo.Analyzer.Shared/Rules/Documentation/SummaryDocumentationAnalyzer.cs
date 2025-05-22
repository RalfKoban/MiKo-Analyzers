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

            if (summaryXmls.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var lazyCommentXml = new Lazy<string>(() => symbol.GetDocumentationCommentXml());
            var lazySummaries = new Lazy<IReadOnlyCollection<string>>(() => CommentExtensions.GetSummaries(lazyCommentXml.Value));

            return AnalyzeSummaries(comment, symbol, summaryXmls, lazyCommentXml, lazySummaries);
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                 DocumentationCommentTriviaSyntax comment,
                                                                 ISymbol symbol,
                                                                 IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                 Lazy<string> commentXml,
                                                                 Lazy<IReadOnlyCollection<string>> summaries)
        {
            return Array.Empty<Diagnostic>();
        }
    }
}