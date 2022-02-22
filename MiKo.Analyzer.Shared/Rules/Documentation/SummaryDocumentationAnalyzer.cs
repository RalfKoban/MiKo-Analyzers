using System;
using System.Collections.Generic;
using System.Linq;

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

        protected Diagnostic AnalyzeSummaryStart(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeStart(symbol, Constants.XmlTag.Summary, summaryXml);

        protected IEnumerable<Diagnostic> AnalyzeSummaryContains(ISymbol symbol, IEnumerable<XmlElementSyntax> summaryXmls, IEnumerable<string> phrases, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var textTokens = summaryXmls.SelectMany(_ => _.DescendantNodes<XmlTextSyntax>()).SelectMany(_ => _.TextTokens);

            return AnalyzeSummaryContains(symbol, textTokens, phrases, comparison);
        }

        protected IEnumerable<Diagnostic> AnalyzeSummaryContains(ISymbol symbol, XmlElementSyntax summaryXml, IEnumerable<string> phrases, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var textTokens = summaryXml.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens);

            return AnalyzeSummaryContains(symbol, textTokens, phrases, comparison);
        }

        protected Diagnostic AnalyzeSummaryEnd(ISymbol symbol, SyntaxNode summaryXml, string phrase)
        {
            var textToken = summaryXml.DescendantNodes<XmlTextSyntax>()
                                      .SelectMany(_ => _.TextTokens)
                                      .LastOrDefault(_ => _.ValueText.IsNullOrWhiteSpace() is false);

            var location = GetFirstLocation(textToken, phrase);
            if (location is null)
            {
                return Issue(symbol.Name, textToken, phrase);
            }

            return null;
        }

        protected virtual Diagnostic SummaryContainsIssue(ISymbol symbol, Location location, string phrase) => Issue(symbol.Name, location, phrase);

        private IEnumerable<Diagnostic> AnalyzeSummaryContains(ISymbol symbol, IEnumerable<SyntaxToken> textTokens, IEnumerable<string> phrases, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var text in textTokens)
            {
                if (text.ValueText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                foreach (var phrase in phrases)
                {
                    var trimmedPhrase = phrase.Trim();

                    var locations = GetAllLocations(text, trimmedPhrase, comparison);

                    foreach (var location in locations)
                    {
                        yield return SummaryContainsIssue(symbol, location, trimmedPhrase);
                    }
                }
            }
        }
    }
}