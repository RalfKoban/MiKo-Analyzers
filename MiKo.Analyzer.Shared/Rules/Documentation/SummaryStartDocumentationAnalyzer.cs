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

        private enum AnalysisResult
        {
            Completed = 0,
            Continue = 1,
        }

        protected virtual Diagnostic NonTextStartIssue(ISymbol symbol, SyntaxNode node) => StartIssue(symbol, node.GetLocation());

        protected virtual Diagnostic TextStartIssue(ISymbol symbol, Location location) => StartIssue(symbol, location);

        protected virtual Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location);

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, count = summaryXmls.Count; index < count; index++)
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

            foreach (var node in xml.DescendantNodes())
            {
                switch (node)
                {
                    case XmlElementStartTagSyntax startTag:
                    {
                        if (AnalyzeStartTag(symbol, tag, startTag, out var issue) is AnalysisResult.Completed)
                        {
                            return issue;
                        }

                        continue;
                    }

                    case XmlElementEndTagSyntax endTag:
                    {
                        if (AnalyzeEndTag(symbol, endTag, out var issue) is AnalysisResult.Completed)
                        {
                            return issue;
                        }

                        continue;
                    }

                    case XmlNameSyntax _:
                    case XmlElementSyntax e when e.GetName() is Constants.XmlTag.Para:
                    case XmlEmptyElementSyntax ee when ee.GetName() is Constants.XmlTag.Para:
                    {
                        continue; // skip over the start tag and name syntax
                    }

                    case XmlTextSyntax text:
                    {
                        if (AnalyzeText(symbol, text, out var issue) is AnalysisResult.Completed)
                        {
                            return issue;
                        }

                        continue;
                    }

                    default:
                        return NonTextStartIssue(symbol, node); // it's no text, so it must be something different
                }
            }

            // nothing to report
            return null;
        }

        private AnalysisResult AnalyzeStartTag(ISymbol symbol, string tag, XmlElementStartTagSyntax startTag, out Diagnostic issue)
        {
            var tagName = startTag.GetName();

            if (tagName == tag || tagName is Constants.XmlTag.Para)
            {
                // skip over the start tag and name syntax
                issue = null;

                return AnalysisResult.Continue;
            }

            // it's no text, so it must be something different
            issue = NonTextStartIssue(symbol, startTag);

            return AnalysisResult.Completed;
        }

        private AnalysisResult AnalyzeEndTag(ISymbol symbol, XmlElementEndTagSyntax endTag, out Diagnostic issue)
        {
            var tagName = endTag.GetName();

            if (tagName is Constants.XmlTag.Para)
            {
                // skip over the start tag and name syntax
                issue = null;

                return AnalysisResult.Continue;
            }

            if (endTag.Parent is XmlElementSyntax element)
            {
                if (ConsiderEmptyTextAsIssue(symbol))
                {
                    // it's an empty text
                    issue = TextStartIssue(symbol, element.GetContentsLocation());

                    return AnalysisResult.Completed;
                }

                issue = null;

                return AnalysisResult.Continue;
            }

            // it's no text, so it must be something different
            issue = NonTextStartIssue(symbol, endTag);

            return AnalysisResult.Completed;
        }

        private AnalysisResult AnalyzeText(ISymbol symbol, XmlTextSyntax text, out Diagnostic issue)
        {
            // report the location of the first word(s) via the corresponding text token
            var textTokens = text.TextTokens;

            for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
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
                    var start = valueText.IndexOf(problematicText, comparison);
                    var end = start + problematicText.Length; // find end position for underlining

                    var location = token.GetLocationWithOffset(start, end);

                    issue = TextStartIssue(symbol, location);

                    return AnalysisResult.Completed;
                }

                // it's a valid text, so we quit
                issue = null;

                return AnalysisResult.Completed;
            }

            // we found a completely empty /// line, so ignore it
            issue = null;

            return AnalysisResult.Continue;
        }
    }
}