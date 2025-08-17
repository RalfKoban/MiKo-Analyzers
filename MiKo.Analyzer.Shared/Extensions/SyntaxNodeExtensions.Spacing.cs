using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s that are related to spacing.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        internal static int GetPositionWithinStartLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetPositionWithinEndLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinEndLine();

        internal static int GetStartingLine(this SyntaxNode value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxNode value) => value.GetLocation().GetEndingLine();

        internal static LinePosition GetStartPosition(this SyntaxNode value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxNode value) => value.GetLocation().GetEndPosition();

        internal static bool HasTrailingEndOfLine(this SyntaxNode value) => value != null && value.GetTrailingTrivia().HasEndOfLine();

        internal static bool IsOnSameLineAs(this SyntaxNode value, SyntaxNode other) => value?.GetStartingLine() == other?.GetStartingLine();

        internal static bool IsOnSameLineAs(this SyntaxNode value, in SyntaxToken other) => value?.GetStartingLine() == other.GetStartingLine();

        internal static bool IsOnSameLineAs(this SyntaxNode value, in SyntaxNodeOrToken other) => value?.GetStartingLine() == other.GetStartingLine();

        internal static T WithAdditionalLeadingTrivia<T>(this T value, in SyntaxTriviaList trivia) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivia));
        }

        internal static T WithAdditionalLeadingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().Add(trivia));
        }

        internal static T WithAdditionalLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivia));
        }

        internal static T WithAdditionalTrailingTrivia<T>(this T value, in SyntaxTriviaList trivia) where T : SyntaxNode
        {
            return value.WithTrailingTrivia(value.GetTrailingTrivia().AddRange(trivia));
        }

        internal static T WithAdditionalTrailingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
        {
            return value.WithTrailingTrivia(value.GetTrailingTrivia().Add(trivia));
        }

        internal static T WithAdditionalTrailingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
        {
            return value.WithTrailingTrivia(value.GetTrailingTrivia().AddRange(trivia));
        }

        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithFirstLeadingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
        {
            // Attention: leading trivia contains XML comments, so we have to keep them!
            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count > 0)
            {
                // remove leading end-of-line as otherwise we would have multiple empty lines left over
                if (leadingTrivia[0].IsEndOfLine())
                {
                    leadingTrivia = leadingTrivia.RemoveAt(0);
                }

                return value.WithLeadingTrivia(leadingTrivia.Insert(0, trivia));
            }

            return value.WithLeadingTrivia(trivia);
        }

        internal static T WithFirstLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
        {
            // Attention: leading trivia contains XML comments, so we have to keep them!
            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count > 0)
            {
                // remove leading end-of-line as otherwise we would have multiple empty lines left over
                if (leadingTrivia[0].IsEndOfLine())
                {
                    leadingTrivia = leadingTrivia.RemoveAt(0);
                }

                return value.WithLeadingTrivia(leadingTrivia.InsertRange(0, trivia));
            }

            return value.WithLeadingTrivia(trivia);
        }

        internal static T WithIndentation<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticMarker); // use elastic one to allow formatting to be done automatically

        internal static T WithAdditionalLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithAdditionalLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static bool HasLeadingEmptyLine(this SyntaxNode value)
        {
            var trivia = value.GetLeadingTrivia();

            if (trivia.Count > 1)
            {
                if (trivia[0].IsEndOfLine())
                {
                    return true;
                }

                if (trivia[0].IsWhiteSpace() && trivia.Count > 1 && trivia[1].IsEndOfLine())
                {
                    return true;
                }
            }

            return false;
        }

        internal static T WithLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again

        internal static T WithLeadingEndOfLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithLeadingSpace<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithTrailingSpace<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithTrailingSpaces<T>(this T value, in int spaces) where T : SyntaxNode => value.WithTrailingTrivia(Enumerable.Repeat(SyntaxFactory.ElasticSpace, spaces)); // use elastic one to allow formatting to be done automatically

        internal static T WithAdditionalLeadingSpaces<T>(this T value, in int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            var currentSpaces = value.GetPositionWithinStartLine();

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static T WithAdditionalLeadingSpacesAtEnd<T>(this T value, in int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            return value.WithAdditionalLeadingTrivia(WhiteSpaces(additionalSpaces));
        }

        internal static T WithAdditionalLeadingSpacesOnDescendants<T>(this T value, IReadOnlyCollection<SyntaxNodeOrToken> descendants, int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            if (descendants.Count is 0)
            {
                return value;
            }

            return value.ReplaceSyntax(
                                   descendants.Where(_ => _.IsNode).Select(_ => _.AsNode()),
                                   (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                   descendants.Where(_ => _.IsToken).Select(_ => _.AsToken()),
                                   (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                   Array.Empty<SyntaxTrivia>(),
                                   (original, rewritten) => rewritten);
        }

        internal static T WithLeadingSpaces<T>(this T value, in int count) where T : SyntaxNode
        {
            if (count <= 0)
            {
                return value;
            }

            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count is 0)
            {
                return value.WithLeadingTrivia(WhiteSpaces(count));
            }

            // re-construct leading comment with correct amount of spaces but keep comments
            // (so we have to find out each white-space trivia and have to replace it with the correct amount of spaces)
            var finalTrivia = leadingTrivia.ToArray();

            var resetFinalTrivia = false;

            for (int index = 0, length = finalTrivia.Length; index < length; index++)
            {
                var trivia = finalTrivia[index];

                if (trivia.IsWhiteSpace())
                {
                    finalTrivia[index] = WhiteSpaces(count);
                }

                if (trivia.IsComment())
                {
                    resetFinalTrivia = true;

                    // we do not need to adjust further as we found a comment and have to fix them based on their specific lines
                    break;
                }
            }

            if (resetFinalTrivia)
            {
                finalTrivia = CalculateWhitespaceTriviaWithComment(count, leadingTrivia.ToArray());
            }

            return value.WithLeadingTrivia(finalTrivia);
        }

        internal static T WithTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.WithLeadingTriviaFrom(node)
                                                                                                        .WithTrailingTriviaFrom(node);

        internal static T WithTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode => value.WithLeadingTriviaFrom(token)
                                                                                                             .WithTrailingTriviaFrom(token);

        internal static T WithAdditionalLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithAdditionalLeadingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithLeadingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithTrailingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetTrailingTrivia();

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static T WithTrailingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static T WithoutLeadingEndOfLine<T>(this T value) where T : SyntaxNode
        {
            var trivia = value.GetLeadingTrivia();

            if (trivia.Count > 0 && trivia[0].IsEndOfLine())
            {
                return value.WithLeadingTrivia(trivia.RemoveAt(0));
            }

            return value;
        }

        internal static XmlElementSyntax WithTagsOnSeparateLines(this XmlElementSyntax value)
        {
            var contents = value.Content;

            var updateStartTag = true;
            var updateEndTag = true;

            if (contents.FirstOrDefault() is XmlTextSyntax firstText)
            {
                if (firstText.HasLeadingTrivia)
                {
                    updateStartTag = false;
                }
                else
                {
                    var textTokens = firstText.TextTokens;
                    var length = textTokens.Count;

                    if (length >= 2)
                    {
                        var firstToken = textTokens[0];
                        var nextToken = textTokens[1];

                        if (firstToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            updateStartTag = false;
                        }
                        else if (nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && firstToken.ValueText.IsNullOrWhiteSpace())
                        {
                            updateStartTag = false;
                        }
                    }
                }
            }

            if (contents.LastOrDefault() is XmlTextSyntax lastText)
            {
                var textTokens = lastText.TextTokens;
                var length = textTokens.Count;

                if (length >= 2)
                {
                    var lastToken = textTokens[length - 1];
                    var secondLastToken = textTokens[length - 2];

                    if (lastToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        updateEndTag = false;
                    }
                    else if (secondLastToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && lastToken.ValueText.IsNullOrWhiteSpace())
                    {
                        updateEndTag = false;
                    }
                }
            }

            if (updateStartTag)
            {
                value = value.WithStartTag(value.StartTag.WithTrailingXmlComment());
            }

            if (updateEndTag)
            {
                value = value.WithEndTag(value.EndTag.WithLeadingXmlComment());
            }

            return value;
        }

        internal static T WithAdditionalTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithAdditionalTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingNewLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use an elastic one to enforce the line break

        internal static T WithoutTrailingSpaces<T>(this T value) where T : SyntaxNode
        {
            var trivia = value.GetTrailingTrivia();
            var triviaCount = trivia.Count;

            if (triviaCount <= 0)
            {
                return value;
            }

            var i = triviaCount - 1;

            for (; i > -1; i--)
            {
                if (trivia[i].IsKind(SyntaxKind.WhitespaceTrivia) is false)
                {
                    break;
                }
            }

            return value.WithTrailingTrivia(i > 0 ? trivia.Take(i) : SyntaxTriviaList.Empty);
        }

        private static SyntaxTrivia[] CalculateWhitespaceTriviaWithComment(in int count, in SyntaxTrivia[] finalTrivia)
        {
            var triviaGroupedByLines = finalTrivia.GroupBy(_ => _.GetStartingLine());

            foreach (var triviaGroup in triviaGroupedByLines)
            {
                var trivia1 = triviaGroup.ElementAt(0);

                if (trivia1.IsWhiteSpace())
                {
                    var index1 = finalTrivia.IndexOf(trivia1);
                    var spaces = count;

                    if (triviaGroup.MoreThan(1))
                    {
                        var trivia2 = triviaGroup.ElementAt(1);

                        if (trivia2.IsMultiLineComment())
                        {
                            var commentLength = trivia2.FullSpan.Length;
                            var remainingSpaces = triviaGroup.Skip(2).Sum(_ => _.FullSpan.Length);

                            spaces = count - commentLength - remainingSpaces;

                            if (spaces < 0)
                            {
                                // it seems we want to remove some spaces, so 'count' is already correct
                                spaces = count;
                            }
                        }
                    }

                    finalTrivia[index1] = WhiteSpaces(spaces);
                }
            }

            return finalTrivia;
        }

        private static SyntaxTrivia WhiteSpaces(in int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}