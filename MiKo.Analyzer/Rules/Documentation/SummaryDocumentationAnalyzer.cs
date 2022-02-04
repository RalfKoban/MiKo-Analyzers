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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var summaryXmls = symbol.GetDocumentationCommentTriviaSyntax().GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeSummary(symbol, summaryXml);
            }
        }

        protected virtual bool SummaryHasIssue(string summary, out XXX issue)
        {
            issue = null;

            return false;
        }

        private Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml)
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
                                    return Issue(node); // it's no text, so it must be something different
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
                                var summary = textToken.ValueText;

                                if (summary.IsNullOrWhiteSpace())
                                {
                                    // we found the first but empty /// line, so ignore it
                                    continue;
                                }

                                // we found some text
                                var hasIssue = SummaryHasIssue(summary, out var value);
                                if (hasIssue)
                                {
                                    // we have an issue, so report the position
                                    var position = summary.IndexOf(value, StringComparison.Ordinal);
                                    var start = textToken.SpanStart + position; // find start position of first word for underlining
                                    var end = start + value.Length; // find end position of first word
                                    var location = Location.Create(textToken.SyntaxTree, TextSpan.FromBounds(start, end));

                                    return Issue(symbol.Name, location);
                                }

                                // no issue, so simply quit
                                return null;
                            }

                            // we found a completely empty /// line, so ignore it
                            continue;
                        }

                    default:
                        return Issue(node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }
    }
}