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

            if (summaryXmls.Count > 0)
            {
                var lazyCommentXml = new Lazy<string>(() => symbol.GetDocumentationCommentXml());
                var lazySummaries = new Lazy<string[]>(() => CommentExtensions.GetSummaries(lazyCommentXml.Value));

                return AnalyzeSummaries(comment, symbol, summaryXmls, lazyCommentXml, lazySummaries);
            }

            // only run for partial type
            if (symbol is ITypeSymbol t && t.IsPartial())
            {
                var contentXmls = comment.GetContentXmls();

                if (contentXmls.Count > 0)
                {
                    var lazyCommentXml = new Lazy<string>(() => symbol.GetDocumentationCommentXml());
                    var lazyContents = new Lazy<string[]>(() => CommentExtensions.GetContents(lazyCommentXml.Value));

                    return AnalyzeContents(comment, symbol, contentXmls, lazyCommentXml, lazyContents);
                }
            }

            return Array.Empty<Diagnostic>();
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                 DocumentationCommentTriviaSyntax comment,
                                                                 ISymbol symbol,
                                                                 IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                 Lazy<string> commentXml,
                                                                 Lazy<string[]> summaries)
        {
            return Array.Empty<Diagnostic>();
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeContents(
                                                                DocumentationCommentTriviaSyntax comment,
                                                                ISymbol symbol,
                                                                IReadOnlyList<XmlElementSyntax> contentXmls,
                                                                Lazy<string> commentXml,
                                                                Lazy<string[]> contents)
        {
            return Array.Empty<Diagnostic>();
        }
    }
}