using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected SummaryDocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(diagnosticId, symbolKind)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeSummaries(symbol, compilation, commentXml, comment);

        protected IEnumerable<Diagnostic> AnalyzeSummaries(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaries = CommentExtensions.GetSummaries(commentXml);

            return summaries.Count == 0
                   ? Array.Empty<Diagnostic>()
                   : AnalyzeSummary(symbol, compilation, summaries, comment);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment) => Array.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeSummariesStart(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            switch (summaryXmls.Count)
            {
                case 0:
                    return Array.Empty<Diagnostic>();

                case 1:
                {
                    var issue = AnalyzeTextStart(symbol, summaryXmls[0]);

                    return issue is null
                           ? Array.Empty<Diagnostic>()
                           : new[] { issue };
                }

                default:
                    return AnalyzeSummaries(symbol, summaryXmls);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSummaries(ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls)
        {
            var count = summaryXmls.Count;

            for (var index = 0; index < count; index++)
            {
                var issue = AnalyzeTextStart(symbol, summaryXmls[index]);

                if (issue != null)
                {
                    yield return issue;
                }
            }
        }
    }
}