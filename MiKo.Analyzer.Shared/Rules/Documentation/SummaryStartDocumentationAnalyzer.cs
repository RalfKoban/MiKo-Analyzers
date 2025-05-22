using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryStartDocumentationAnalyzer : SummaryDocumentationAnalyzer
    {
        protected SummaryStartDocumentationAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected virtual Diagnostic NonTextStartIssue(ISymbol symbol, SyntaxNode node) => StartIssue(symbol, node.GetLocation());

        protected virtual Diagnostic TextStartIssue(ISymbol symbol, Location location) => StartIssue(symbol, location);

        protected virtual Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location);

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<IReadOnlyCollection<string>> summaries)
        {
            var count = summaryXmls.Count;

            List<Diagnostic> issues = null;

            for (var index = 0; index < count; index++)
            {
                var issue = AnalyzeTextStart(symbol, summaryXmls[index]);

                if (issue is null)
                {
                    continue;
                }

                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(issue);
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        protected Diagnostic AnalyzeTextStart(ISymbol symbol, XmlElementSyntax xml)
        {
            var tag = xml.StartTag.GetName();

            var descendantNodes = xml.DescendantNodes();

            foreach (var node in descendantNodes)
            {
                switch (node)
                {
                    case XmlElementStartTagSyntax startTag:
                    {
                        var tagName = startTag.GetName();

                        if (tagName == tag || tagName is Constants.XmlTag.Para)
                        {
                            continue; // skip over the start tag and name syntax
                        }

                        return NonTextStartIssue(symbol, node); // it's no text, so it must be something different
                    }

                    case XmlElementEndTagSyntax endTag:
                    {
                        var tagName = endTag.GetName();

                        if (tagName is Constants.XmlTag.Para)
                        {
                            continue; // skip over the start tag and name syntax
                        }

                        if (endTag.Parent is XmlElementSyntax element)
                        {
                            if (ConsiderEmptyTextAsIssue(symbol))
                            {
                                return TextStartIssue(symbol, element.GetContentsLocation()); // it's an empty text
                            }

                            continue;
                        }

                        return NonTextStartIssue(symbol, node); // it's no text, so it must be something different
                    }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() is Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() is Constants.XmlTag.Para:
                        continue; // skip over the start tag and name syntax

                    case XmlTextSyntax text:
                    {
                        // report the location of the first word(s) via the corresponding text token
                        var textTokens = text.TextTokens;

                        // keep in local variable to avoid multiple requests (see Roslyn implementation)
                        var textTokensCount = textTokens.Count;

                        for (var index = 0; index < textTokensCount; index++)
                        {
                            var token = textTokens[index];

                            if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                            {
                                continue;
                            }

                            var valueText = token.ValueText;

                            if (valueText.IsNullOrWhiteSpace())
                            {
                                // we found the first but empty /// line, so ignore it
                                continue;
                            }

                            if (valueText.Length is 1 && Constants.Comments.Delimiters.Contains(valueText[0]))
                            {
                                // this is a dot or something directly after the XML tag, so ignore that
                                continue;
                            }

                            // we found some text
                            if (AnalyzeTextStart(symbol, valueText, out var problematicText, out var comparison))
                            {
                                // it's no valid text, so we have an issue
                                var position = valueText.IndexOf(problematicText, comparison);

                                var start = token.SpanStart + position; // find start position for underlining
                                var end = start + problematicText.Length; // find end position for underlining

                                var location = CreateLocation(token, start, end);

                                return TextStartIssue(symbol, location);
                            }

                            // it's a valid text, so we quit
                            return null;
                        }

                        // we found a completely empty /// line, so ignore it
                        continue;
                    }

                    default:
                        return NonTextStartIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }
    }
}