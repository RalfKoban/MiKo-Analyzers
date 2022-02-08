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

        protected virtual Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected Diagnostic AnalyzeSummaryStart(ISymbol symbol, SyntaxNode summaryXml)
        {
            var elementsToSkip = 0;
            var analyzedFirstText = false;

            var descendantNodes = summaryXml.DescendantNodes();
            foreach (var node in descendantNodes.TakeWhile(_ => analyzedFirstText is false))
            {
                elementsToSkip++;

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

                                analyzedFirstText = true;

                                var issue = SummaryStartIssue(symbol, textToken);
                                if (issue != null)
                                {
                                    return issue;
                                }
                            }

                            // we found a completely empty /// line, so ignore it
                            continue;
                        }

                    default:
                        return SummaryStartIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            return AnalyzeSummaryContinue(symbol, descendantNodes.Skip(elementsToSkip));
        }

        protected virtual Diagnostic AnalyzeSummaryContinue(ISymbol symbol, IEnumerable<SyntaxNode> remainingNodes) => null; // nothing to report

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