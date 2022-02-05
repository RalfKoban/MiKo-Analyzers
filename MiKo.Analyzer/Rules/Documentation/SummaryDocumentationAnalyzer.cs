using System;
using System.Collections.Generic;

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

            var start = textToken.SpanStart + position; // find start position for underlining
            var end = start + value.Length; // find end position

            var location = Location.Create(textToken.SyntaxTree, TextSpan.FromBounds(start, end));

            return location;
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var summaryXmls = symbol.GetDocumentationCommentTriviaSyntax().GetSummaryXmls();

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
                                    return SummaryIssue(symbol, node); // it's no text, so it must be something different
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

                                return SummaryIssue(symbol, textToken);
                            }

                            // we found a completely empty /// line, so ignore it
                            continue;
                        }

                    default:
                        return SummaryIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }

        protected virtual Diagnostic SummaryIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node);

        protected virtual Diagnostic SummaryIssue(ISymbol symbol, SyntaxToken textToken) => Issue(symbol.Name, textToken);
    }
}