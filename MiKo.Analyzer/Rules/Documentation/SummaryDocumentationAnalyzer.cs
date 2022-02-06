using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected SummaryDocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(diagnosticId, symbolKind)
        {
        }

        protected static Location GetLocation(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var position = textToken.ValueText.IndexOf(value, comparison);

            if (position == -1)
            {
                return null;
            }

            var start = textToken.SpanStart + position; // find start position for underlining
            var end = start + value.Length; // find end position

            return Location.Create(textToken.SyntaxTree, TextSpan.FromBounds(start, end));
        }

        protected static IEnumerable<Location> GetLocations(SyntaxToken textToken, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            var positions = textToken.ValueText.AllIndexesOf(value, comparison);

            foreach (var position in positions)
            {
                var start = textToken.SpanStart + position; // find start position for underlining
                var end = start + value.Length; // find end position

                yield return Location.Create(textToken.SyntaxTree, TextSpan.FromBounds(start, end));
            }
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            return AnalyzeSummary(symbol, symbol.GetDocumentationCommentTriviaSyntax());
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, DocumentationCommentTriviaSyntax documentation)
        {
            return AnalyzeSummary(symbol, documentation.GetSummaryXmls());
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<XmlElementSyntax> summaryXmls)
        {
            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeSummary(symbol, summaryXml);
            }
        }

        protected virtual Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => null;

        protected Diagnostic AnalyzeSummaryStart(ISymbol symbol, SyntaxNode summaryXml)
        {
            var descendantNodes = summaryXml.DescendantNodes();
            foreach (var node in descendantNodes)
            {
                switch (node)
                {
                    case XmlElementStartTagSyntax startTag:
                        {
                            var tagName = startTag.GetName();
                            switch (tagName)
                            {
                                case Constants.XmlTag.Summary:
                                case Constants.XmlTag.Para:
                                    continue; // skip over the start tag and name syntax

                                default:
                                    return SummaryStartIssue(symbol, node); // it's no text, so it must be something different
                            }
                        }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() == Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() == Constants.XmlTag.Para:
                        continue; // skip over the start tag and name syntax

                    case XmlTextSyntax text:
                        {
                            // report the location of the text issue via the corresponding text token
                            foreach (var textToken in text.TextTokens)
                            {
                                if (textToken.ValueText.IsNullOrWhiteSpace())
                                {
                                    // we found the first but empty /// line, so ignore it
                                    continue;
                                }

                                return SummaryStartIssue(symbol, textToken);
                            }

                            // we found a completely empty /// line, so ignore it
                            continue;
                        }

                    default:
                        return SummaryStartIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }

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

        protected virtual Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node);

        protected virtual Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken) => Issue(symbol.Name, textToken);

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

                    var locations = GetLocations(text, trimmedPhrase, comparison);

                    foreach (var location in locations)
                    {
                        yield return SummaryContainsIssue(symbol, location, trimmedPhrase);
                    }
                }
            }
        }
    }
}