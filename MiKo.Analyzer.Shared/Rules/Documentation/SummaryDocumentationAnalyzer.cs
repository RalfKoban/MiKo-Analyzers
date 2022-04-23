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

        protected sealed override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            return AnalyzeSummary(symbol, comment);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            return AnalyzeSummary(symbol, comment.GetSummaryXmls());
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<XmlElementSyntax> summaryXmls)
        {
            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeSummary(symbol, summaryXml);
            }
        }

        protected virtual Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected Diagnostic AnalyzeSummaryStart(ISymbol symbol, SyntaxNode startElement) => AnalyzeStart(symbol, Constants.XmlTag.Summary, startElement);
    }
}