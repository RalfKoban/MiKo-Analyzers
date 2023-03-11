using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            return summaries.Any()
                       ? AnalyzeSummary(symbol, compilation, summaries, comment)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment) => Enumerable.Empty<Diagnostic>();

        protected virtual Diagnostic StartIssue(SyntaxNode node) => Issue(node);

        protected virtual Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location);

        protected virtual bool AnalyzeTextStart(string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = null;
            comparison = StringComparison.Ordinal;

            return false;
        }

        protected Diagnostic AnalyzeTextStart(ISymbol symbol, SyntaxNode summaryXml)
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
                                    return StartIssue(node); // it's no text, so it must be something different
                            }
                        }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() == Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() == Constants.XmlTag.Para:
                        continue; // skip over the start tag and name syntax

                    case XmlTextSyntax text:
                        {
                            // report the location of the first word(s) via the corresponding text token
                            foreach (var textToken in text.TextTokens.Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)))
                            {
                                var valueText = textToken.ValueText;

                                if (valueText.IsNullOrWhiteSpace())
                                {
                                    // we found the first but empty /// line, so ignore it
                                    continue;
                                }

                                // we found some text
                                if (AnalyzeTextStart(valueText, out var problematicText, out var comparison))
                                {
                                    // it's no valid text, so we have an issue
                                    var position = valueText.IndexOf(problematicText, comparison);

                                    var start = textToken.SpanStart + position; // find start position for underlining
                                    var end = start + problematicText.Length; // find end position for underlining

                                    var location = CreateLocation(textToken, start, end);

                                    return StartIssue(symbol, location);
                                }

                                // it's a valid text, so we quit
                                return null;
                            }

                            // we found a completely empty /// line, so ignore it
                            continue;
                        }

                    default:
                        return StartIssue(node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }
    }
}