using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2224";

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.Overloads,
                                                               Constants.XmlTag.Note,
                                                               Constants.XmlTag.Summary,
                                                               Constants.XmlTag.Exception,
                                                               Constants.XmlTag.Param,
                                                               Constants.XmlTag.Para, // (except for <para>-or-</para>)
                                                               Constants.XmlTag.TypeParam,
                                                               Constants.XmlTag.Returns,
                                                               Constants.XmlTag.Value,
                                                               Constants.XmlTag.Remarks,
                                                               Constants.XmlTag.Example,
                                                               Constants.XmlTag.List,
                                                               Constants.XmlTag.Response,
                                                           };

        private static readonly HashSet<string> EmptyTags = new HashSet<string>
                                                                {
                                                                    Constants.XmlTag.Para,
                                                                };

        private static readonly SyntaxKind[] ProblematicSiblingKinds = { SyntaxKind.XmlElementStartTag, SyntaxKind.XmlElementEndTag, SyntaxKind.XmlEmptyElement };

        public MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(comment);

        private static bool IsOnSameLine(XmlTextSyntax text, ICollection<int> lines)
        {
            var textTokens = text.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.Text.IsNullOrWhiteSpace())
                {
                    // ignore that
                    continue;
                }

                var span = token.GetLocation().GetLineSpan();

                if (lines.Contains(span.StartLinePosition.Line) || lines.Contains(span.EndLinePosition.Line))
                {
                    // seems that we have either some text before or after the line
                    return true;
                }
            }

            return false;
        }

        private static bool IsOnSameLine(SyntaxList<XmlNodeSyntax> contents, ICollection<int> lines)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var contentsCount = contents.Count;

            for (var index = 0; index < contentsCount; index++)
            {
                var content = contents[index];

                if (content is XmlTextSyntax text)
                {
                    if (IsOnSameLine(text, lines))
                    {
                        return true;
                    }
                }
                else
                {
                    var line = content.GetStartingLine();

                    if (lines.Contains(line))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsOnSameLine(XmlTextSyntax text, params int[] lines) => IsOnSameLine(text, (ICollection<int>)lines);

        private static bool IsOnSameLine(SyntaxList<XmlNodeSyntax> contents, params int[] lines) => IsOnSameLine(contents, (ICollection<int>)lines);

        private static Dictionary<string, string> CreateProperties(bool onSameLineAsTextBefore, bool onSameLineAsTextAfter)
        {
            var properties = new Dictionary<string, string>();

            if (onSameLineAsTextBefore)
            {
                properties.Add(Constants.AnalyzerCodeFixSharedData.AddSpaceBefore, string.Empty);
            }

            if (onSameLineAsTextAfter)
            {
                properties.Add(Constants.AnalyzerCodeFixSharedData.AddSpaceAfter, string.Empty);
            }

            return properties;
        }

        private IEnumerable<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            var lines = new HashSet<int>();

            var emptyElements = new List<XmlEmptyElementSyntax>();
            var elements = new List<XmlElementSyntax>();

            foreach (var node in comment.DescendantNodes<XmlNodeSyntax>())
            {
                switch (node)
                {
                    case XmlEmptyElementSyntax emptyElement:
                        emptyElements.Add(emptyElement);

                        break;

                    case XmlElementSyntax element:
                        elements.Add(element);

                        break;
                }
            }

            // first loop over the elements to collect all lines that are also required to check for the empty elements, then loop over the empty ones
            return Enumerable.Empty<Diagnostic>()
                             .Concat(elements.SelectMany(_ => AnalyzeXmlElement(_, lines)))
                             .Concat(emptyElements.Select(_ => AnalyzeEmptyXmlElement(_, lines)));
        }

        private IEnumerable<Diagnostic> AnalyzeXmlElement(XmlElementSyntax element, ISet<int> lines)
        {
            var elementName = element.GetName();

            if (Tags.Contains(elementName) is false)
            {
                // that is allowed
                yield break;
            }

            if (elementName == Constants.XmlTag.Para && element.GetTextTrimmed().Equals(Constants.Comments.SpecialOrPhrase.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                // that is allowed
                yield break;
            }

            var startTagLine = element.StartTag.GetStartingLine();
            var endTagLine = element.EndTag.GetStartingLine();

            var startLineOnSeparateLine = lines.Add(startTagLine);
            var endLineOnSeparateLine = lines.Add(endTagLine);

            if (startLineOnSeparateLine)
            {
                if (IsOnSameLine(element.Content, startTagLine))
                {
                    yield return Issue(element.StartTag);
                }
            }
            else
            {
                yield return Issue(element.StartTag);
            }

            if (endLineOnSeparateLine)
            {
                if (IsOnSameLine(element.Content, endTagLine))
                {
                    yield return Issue(element.EndTag);
                }
            }
            else
            {
                yield return Issue(element.EndTag);
            }
        }

        private Diagnostic AnalyzeEmptyXmlElement(XmlEmptyElementSyntax element, ISet<int> lines)
        {
            if (EmptyTags.Contains(element.GetName()))
            {
                var startingLine = element.GetStartingLine();

                var previousSibling = element.PreviousSibling();
                var nextSibling = element.NextSibling();

                if (lines.Add(startingLine))
                {
                    var onSameLineAsTextBefore = previousSibling is XmlTextSyntax previousText && IsOnSameLine(previousText, startingLine);
                    var onSameLineAsTextAfter = nextSibling is XmlTextSyntax nextText && IsOnSameLine(nextText, startingLine);

                    if (onSameLineAsTextBefore || onSameLineAsTextAfter)
                    {
                        var properties = CreateProperties(onSameLineAsTextBefore, onSameLineAsTextAfter);

                        return Issue(element, properties);
                    }
                }
                else
                {
                    // seems like it is already on same line with another one
                    var properties = CreateProperties(previousSibling.IsAnyKind(ProblematicSiblingKinds), nextSibling.IsAnyKind(ProblematicSiblingKinds));

                    return Issue(element, properties);
                }
            }

            return null;
        }
    }
}