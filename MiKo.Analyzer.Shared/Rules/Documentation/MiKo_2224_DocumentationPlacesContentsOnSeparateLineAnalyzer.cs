﻿using System;
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

        private static readonly Pair[] AddSpacesBoth = { new Pair(Constants.AnalyzerCodeFixSharedData.AddSpaceBefore), new Pair(Constants.AnalyzerCodeFixSharedData.AddSpaceAfter) };
        private static readonly Pair[] AddSpacesBefore = { new Pair(Constants.AnalyzerCodeFixSharedData.AddSpaceBefore, string.Empty) };
        private static readonly Pair[] AddSpacesAfter = { new Pair(Constants.AnalyzerCodeFixSharedData.AddSpaceAfter) };

        private static readonly SyntaxKind[] ProblematicSiblingKinds = { SyntaxKind.XmlElementStartTag, SyntaxKind.XmlElementEndTag, SyntaxKind.XmlEmptyElement };

        public MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
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
            List<Diagnostic> results = null;

            foreach (var element in elements)
            {
                foreach (var issue in AnalyzeXmlElement(element, lines))
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(issue);
                }
            }

            foreach (var element in emptyElements)
            {
                var issue = AnalyzeEmptyXmlElement(element, lines);

                if (issue is null)
                {
                    continue;
                }

                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(issue);
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }

        private static bool IsOnSameLine(XmlTextSyntax text, params int[] lines)
        {
            var textTokens = text.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
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

        private static bool IsOnSameLine(in SyntaxList<XmlNodeSyntax> contents, params int[] lines)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, contentsCount = contents.Count; index < contentsCount; index++)
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

        private static Pair[] CreateProperties(in bool onSameLineAsTextBefore, in bool onSameLineAsTextAfter)
        {
            if (onSameLineAsTextBefore)
            {
                return onSameLineAsTextAfter ? AddSpacesBoth : AddSpacesBefore;
            }

            return onSameLineAsTextAfter ? AddSpacesAfter : Array.Empty<Pair>();
        }

        private IEnumerable<Diagnostic> AnalyzeXmlElement(XmlElementSyntax element, HashSet<int> lines)
        {
            var elementName = element.GetName();

            if (Tags.Contains(elementName) is false)
            {
                // that is allowed
                yield break;
            }

            if (elementName is Constants.XmlTag.Para && element.GetTextTrimmed().Equals(Constants.Comments.SpecialOrPhrase.AsSpan(), StringComparison.OrdinalIgnoreCase))
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

        private Diagnostic AnalyzeEmptyXmlElement(XmlEmptyElementSyntax element, HashSet<int> lines)
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