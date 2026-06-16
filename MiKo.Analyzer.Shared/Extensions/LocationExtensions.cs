using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="Location"/>s.
    /// </summary>
    internal static class LocationExtensions
    {
        /// <summary>
        /// Determines whether this location contains another location.
        /// </summary>
        /// <param name="value">
        /// The location to check from.
        /// </param>
        /// <param name="other">
        /// The location to seek.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified location contains the other location; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Contains(this Location value, Location other)
        {
            var valueSpan = value.SourceSpan;
            var otherSpan = other.SourceSpan;

            return valueSpan.Start <= otherSpan.Start && otherSpan.End <= valueSpan.End;
        }

        /// <summary>
        /// Gets the enclosing symbol of type <typeparamref name="T"/> for the specified location.
        /// </summary>
        /// <typeparam name="T">
        /// The type of symbol to retrieve.
        /// </typeparam>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to seek the enclosing symbol.
        /// </param>
        /// <returns>
        /// The enclosing symbol of type <typeparamref name="T"/>, or <see langword="null"/> if no such symbol exists.
        /// </returns>
        internal static T GetEnclosing<T>(this Location value, SemanticModel semanticModel) where T : class, ISymbol
        {
            var node = value.SourceTree?.GetRoot().FindNode(value.SourceSpan);

            return node.GetEnclosingSymbol(semanticModel) as T;
        }

        /// <summary>
        /// Gets the ending line number of the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// The ending line number (zero-based).
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetEndingLine(this Location value) => value.GetEndPosition().Line;

        /// <summary>
        /// Gets the end position of the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// The end position as a <see cref="LinePosition"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetEndPosition(this Location value) => value.GetLineSpan().EndLinePosition;

        /// <summary>
        /// Gets the character position within the end line of the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// The character position within the end line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinEndLine(this Location value) => value.GetEndPosition().Character;

        /// <summary>
        /// Gets the character position within the start line of the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// The character position within the start line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinStartLine(this Location value) => value.GetStartPosition().Character;

        /// <summary>
        /// Gets the starting line number of the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// The starting line number (zero-based).
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetStartingLine(this Location value) => value.GetStartPosition().Line;

        /// <summary>
        /// Gets the start position of the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// The start position as a <see cref="LinePosition"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetStartPosition(this Location value) => value.GetLineSpan().StartLinePosition;

        /// <summary>
        /// Gets the complete word surrounding the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the complete word surrounding the location, or <see langword="null"/> if the location does not have a source tree or the surrounding word cannot be determined.
        /// </returns>
        internal static string GetSurroundingWord(this Location value)
        {
            var tree = value.SourceTree;

            if (tree is null)
            {
                return null;
            }

            const int MaximumCharacters = 200;

            var sourceSpanEnd = value.SourceSpan.End;
            var sourceSpanStart = Math.Max(0, sourceSpanEnd - MaximumCharacters);

            var sourceText = tree.GetText();

            var startUpTextBounds = TextSpan.FromBounds(sourceSpanStart, sourceSpanEnd);
            var startUpText = sourceText.ToString(startUpTextBounds).AsSpan();

            var lastIndexOfFirstSpace = startUpText.LastIndexOfAny(Constants.WhiteSpaceCharacters);

            if (lastIndexOfFirstSpace is -1)
            {
                return null;
            }

            var followUpTextBounds = TextSpan.FromBounds(sourceSpanEnd, Math.Min(sourceSpanEnd + MaximumCharacters, sourceText.Length));
            var followUpText = sourceText.ToString(followUpTextBounds).AsSpan();

            var firstIndexOfNextSpace = followUpText.StartsWith('<') // seems like the comment finished
                                        ? 0
                                        : followUpText.IndexOfAny(Constants.WhiteSpaceCharacters);

            if (firstIndexOfNextSpace is -1)
            {
                return null;
            }

            return sourceText.ToString(TextSpan.FromBounds(sourceSpanStart + lastIndexOfFirstSpace + 1, sourceSpanEnd + firstIndexOfNextSpace));
        }

        /// <summary>
        /// Gets the text content at the specified location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text content at the location, or <see langword="null"/> if the location does not have a source tree.
        /// </returns>
        internal static string GetText(this Location value) => value.SourceTree?.GetText().ToString(value.SourceSpan);

        /// <summary>
        /// Gets a substring of the specified length starting from the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <param name="length">
        /// The number of characters to retrieve.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text of the specified length starting from the location, or <see langword="null"/> if the location does not have a source tree.
        /// </returns>
        internal static string GetText(this Location value, in int length) => GetText(value, 0, length);

        /// <summary>
        /// Gets a substring of the specified length starting from the location.
        /// </summary>
        /// <param name="value">
        /// The location to examine.
        /// </param>
        /// <param name="startOffset">
        /// The offset of characters to start with.
        /// </param>
        /// <param name="length">
        /// The number of characters to retrieve.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text of the specified length starting from the location, or <see langword="null"/> if the location does not have a source tree.
        /// </returns>
        internal static string GetText(this Location value, in int startOffset, in int length)
        {
            var tree = value.SourceTree;

            if (tree is null)
            {
                return null;
            }

            var start = value.SourceSpan.Start + startOffset;

            return tree.GetText().ToString(TextSpan.FromBounds(start, start + length));
        }

        /// <summary>
        /// Gets the location of the first occurrence of the specified value within the token's text or <see langword="null"/> if the value is not found.
        /// </summary>
        /// <param name="value">
        /// The syntax token to search.
        /// </param>
        /// <param name="text">
        /// The text to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the first occurrence, or <see langword="null"/> if not found.
        /// </returns>
        internal static Location GetFirstLocation(this in SyntaxToken value, string text, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return text.GetLocation(value.SyntaxTree, value.SpanStart, value.ValueText.IndexOf(text, comparison), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the first occurrence of the specified value within the trivia's text or <see langword="null"/> if the value is not found.
        /// </summary>
        /// <param name="value">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="text">
        /// The text to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the first occurrence, or <see langword="null"/> if not found.
        /// </returns>
        internal static Location GetFirstLocation(this in SyntaxTrivia value, string text, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return text.GetLocation(value.SyntaxTree, value.SpanStart, value.ToFullString().IndexOf(text, comparison), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the last occurrence of the specified value within the trivia's text or <see langword="null"/> if the value is not found.
        /// </summary>
        /// <param name="value">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="character">
        /// The value to seek.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the last occurrence, or <see langword="null"/> if not found.
        /// </returns>
        internal static Location GetLastLocation(this in SyntaxTrivia value, in char character, in int startOffset = 0, in int endOffset = 0)
        {
            return character.GetLocation(value.SyntaxTree, value.SpanStart, value.ToFullString().LastIndexOf(character), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the last occurrence of the specified value within the token's text or <see langword="null"/> if the value is not found.
        /// </summary>
        /// <param name="value">
        /// The syntax token to search.
        /// </param>
        /// <param name="text">
        /// The text to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// The location of the last occurrence, or <see langword="null"/> if not found.
        /// </returns>
        internal static Location GetLastLocation(this in SyntaxToken value, string text, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return text.GetLocation(value.SyntaxTree, value.SpanStart, value.ValueText.LastIndexOf(text, comparison), startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified value within the token's text.
        /// </summary>
        /// <param name="value">
        /// The syntax token to search.
        /// </param>
        /// <param name="text">
        /// The text to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        internal static IReadOnlyList<Location> GetAllLocations(this in SyntaxToken value, string text, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(value.ValueText, value.SyntaxTree, value.SpanStart, text, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified values within the token's text.
        /// </summary>
        /// <param name="value">
        /// The syntax token to search.
        /// </param>
        /// <param name="texts">
        /// The values to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        internal static IReadOnlyList<Location> GetAllLocations(this in SyntaxToken value, in ReadOnlySpan<string> texts, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(value.ValueText, value.SyntaxTree, value.SpanStart, texts, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified value within the trivia's text.
        /// </summary>
        /// <param name="value">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="text">
        /// The text to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        internal static IReadOnlyList<Location> GetAllLocations(this in SyntaxTrivia value, string text, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(value.ToFullString(), value.SyntaxTree, value.SpanStart, text, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified values within the trivia's text.
        /// </summary>
        /// <param name="value">
        /// The syntax trivia to search.
        /// </param>
        /// <param name="texts">
        /// The values to seek.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found.
        /// </returns>
        internal static IReadOnlyList<Location> GetAllLocations(this in SyntaxTrivia value, in ReadOnlySpan<string> texts, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(value.ToFullString(), value.SyntaxTree, value.SpanStart, texts, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the locations of all occurrences of the specified value within the token's text where the next character passes the validation.
        /// </summary>
        /// <param name="value">
        /// The syntax token to search.
        /// </param>
        /// <param name="text">
        /// The text to seek.
        /// </param>
        /// <param name="nextCharValidationCallback">
        /// A callback to validate the character following the found value.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration members that specifies the <see cref="string"/> comparison mode to use.
        /// The default is <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A collection of locations for all occurrences found that pass validation.
        /// </returns>
        internal static IReadOnlyList<Location> GetAllLocations(this in SyntaxToken value, string text, Func<char, bool> nextCharValidationCallback, in StringComparison comparison = StringComparison.Ordinal, in int startOffset = 0, in int endOffset = 0)
        {
            return GetAllLocations(value.ValueText, value.SyntaxTree, value.SpanStart, text, nextCharValidationCallback, comparison, startOffset, endOffset);
        }

        /// <summary>
        /// Gets the location of the first text issue within the specified XML nodes.
        /// </summary>
        /// <param name="source">
        /// The XML nodes to search.
        /// </param>
        /// <returns>
        /// The location of the first text issue found.
        /// </returns>
        internal static Location GetFirstTextIssueLocation(this in SyntaxList<XmlNodeSyntax> source)
        {
            var node = source[0];

            if (node is XmlTextSyntax text)
            {
                var textTokens = text.TextTokens;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                    {
                        var tokenText = token.ValueText;
                        var leadingWhitespaces = tokenText.CountLeadingWhitespaces();

                        var location = leadingWhitespaces < tokenText.Length // check for spaces
                                       ? token.GetLocationWithOffset(0, leadingWhitespaces)
                                       : token.GetLocation();

                        return location;
                    }
                }
            }

            return node.GetLocation();
        }

        /// <summary>
        /// Gets a location from a syntax trivia list.
        /// </summary>
        /// <param name="source">
        /// The syntax trivia list.
        /// </param>
        /// <returns>
        /// A location that spans the entire trivia list, or <see cref="Location.None"/> if the list is empty.
        /// </returns>
        internal static Location GetLocation(this in SyntaxTriviaList source)
        {
            if (source.Count > 0)
            {
                var span = source.FullSpan;

                return GetLocation(source[0].SyntaxTree, span.Start, span.End);
            }

            return Location.None;
        }

        /// <summary>
        /// Gets a location from a syntax node with specified bounds.
        /// </summary>
        /// <param name="value">
        /// The syntax node.
        /// </param>
        /// <param name="start">
        /// The start position of the location.
        /// </param>
        /// <param name="end">
        /// The end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        internal static Location GetLocation(this SyntaxNode value, in int start, in int end) => GetLocation(value.SyntaxTree, start, end);

        /// <summary>
        /// Gets a location from a syntax token with specified bounds.
        /// </summary>
        /// <param name="value">
        /// The syntax token.
        /// </param>
        /// <param name="start">
        /// The start position of the location.
        /// </param>
        /// <param name="end">
        /// The end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        internal static Location GetLocation(this in SyntaxToken value, in int start, in int end) => GetLocation(value.SyntaxTree, start, end);

        /// <summary>
        /// Gets a location from a syntax tree with specified bounds.
        /// </summary>
        /// <param name="value">
        /// The syntax tree.
        /// </param>
        /// <param name="start">
        /// The start position of the location.
        /// </param>
        /// <param name="end">
        /// The end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        internal static Location GetLocation(this SyntaxTree value, in int start, in int end) => Location.Create(value, TextSpan.FromBounds(start, end));

        /// <summary>
        /// Gets a location for a character within a syntax tree.
        /// </summary>
        /// <param name="value">
        /// The character value.
        /// </param>
        /// <param name="syntaxTree">
        /// The syntax tree.
        /// </param>
        /// <param name="spanStart">
        /// The start position of the containing span.
        /// </param>
        /// <param name="position">
        /// The position of the character relative to the span start, or <c>-1</c> if not found.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A location for the character, or <see langword="null"/> if the position is <c>-1</c> or the calculated bounds are invalid.
        /// </returns>
        internal static Location GetLocation(this in char value, SyntaxTree syntaxTree, in int spanStart, in int position, in int startOffset = 0, in int endOffset = 0)
        {
            if (position is -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + sizeof(char) - startOffset - endOffset; // find end position

            if (end < start)
            {
                // seems we did not find a proper location here
                return null;
            }

            return GetLocation(syntaxTree, start, end);
        }

        /// <summary>
        /// Gets a location for a <see cref="string"/> within a syntax tree.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> value.
        /// </param>
        /// <param name="syntaxTree">
        /// The syntax tree.
        /// </param>
        /// <param name="spanStart">
        /// The start position of the containing span.
        /// </param>
        /// <param name="position">
        /// The position of the <see cref="string"/> relative to the span start, or <c>-1</c> if not found.
        /// </param>
        /// <param name="startOffset">
        /// The offset to apply to the start position.
        /// The default is <c>0</c>.
        /// </param>
        /// <param name="endOffset">
        /// The offset to apply to the end position.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// A location for the <see cref="string"/>, or <see langword="null"/> if the position is <c>-1</c> or the calculated bounds are invalid.
        /// </returns>
        internal static Location GetLocation(this string value, SyntaxTree syntaxTree, in int spanStart, in int position, in int startOffset = 0, in int endOffset = 0)
        {
            if (position is -1)
            {
                return null;
            }

            var start = spanStart + position + startOffset; // find start position for underlining
            var end = start + value.Length - startOffset - endOffset; // find end position

            if (end < start)
            {
                // seems we did not find a proper location here
                return null;
            }

            return GetLocation(syntaxTree, start, end);
        }

        /// <summary>
        /// Gets a location from a syntax token with specified offsets.
        /// </summary>
        /// <param name="value">
        /// The syntax token.
        /// </param>
        /// <param name="offsetStart">
        /// The offset start position of the location.
        /// </param>
        /// <param name="offsetEnd">
        /// The offset end position of the location.
        /// </param>
        /// <returns>
        /// A location that spans from the start to the end position.
        /// </returns>
        internal static Location GetLocationWithOffset(this in SyntaxToken value, in int offsetStart, in int offsetEnd)
        {
            var spanStart = value.SpanStart;

            return value.GetLocation(spanStart + offsetStart, spanStart + offsetEnd);
        }

        /// <summary>
        /// Determines whether this location intersects with another location.
        /// </summary>
        /// <param name="value">
        /// The location to check from.
        /// </param>
        /// <param name="other">
        /// The location to check for intersection.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified location intersects with the other location; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IntersectsWith(this Location value, Location other)
        {
            var valueSpan = value.SourceSpan;
            var otherSpan = other.SourceSpan;

            return valueSpan.Start < otherSpan.End && otherSpan.Start < valueSpan.End;
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, in int spanStart, string value, in StringComparison comparison, in int startOffset, in int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            if (textLength < value.Length)
            {
                // nothing to inspect as the text is too short
                return Array.Empty<Location>();
            }

            var allIndices = text.AllIndicesOf(value, comparison);

            if (allIndices.Length is 0)
            {
                // nothing to inspect
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(syntaxTree, spanStart, value, startOffset, endOffset, allIndices);
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, in int spanStart, in ReadOnlySpan<string> values, in StringComparison comparison, in int startOffset, in int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(text, syntaxTree, spanStart, values, comparison, startOffset, endOffset, textLength);
        }

        private static IReadOnlyList<Location> GetAllLocations(string text, SyntaxTree syntaxTree, in int spanStart, string value, Func<char, bool> nextCharValidationCallback, in StringComparison comparison, in int startOffset, in int endOffset)
        {
            var textLength = text.Length;

            if (textLength <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
            {
                // nothing to inspect as the text is too short and consists of whitespaces only
                return Array.Empty<Location>();
            }

            if (textLength < value.Length)
            {
                // nothing to inspect as the text is too short
                return Array.Empty<Location>();
            }

            var allIndices = text.AllIndicesOf(value, comparison);

            if (allIndices.Length is 0)
            {
                // nothing to inspect
                return Array.Empty<Location>();
            }

            return GetAllLocationsWithLoop(text, syntaxTree, spanStart, value, nextCharValidationCallback, startOffset, endOffset, textLength, allIndices);
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(SyntaxTree syntaxTree, in int spanStart, string value, in int startOffset, in int endOffset, in ReadOnlySpan<int> allIndices)
        {
            List<Location> alreadyReportedLocations = null;
            List<Location> results = null;

            for (int index = 0, length = allIndices.Length; index < length; index++)
            {
                var position = allIndices[index];

                var location = value.GetLocation(syntaxTree, spanStart, position, startOffset, endOffset);

                if (location is null)
                {
                    continue;
                }

                if (alreadyReportedLocations is null)
                {
                    alreadyReportedLocations = new List<Location>(1);
                }
                else
                {
                    if (alreadyReportedLocations.Exists(location.IntersectsWith))
                    {
                        // already reported, so ignore it
                        continue;
                    }
                }

                alreadyReportedLocations.Add(location);

                if (results is null)
                {
                    results = new List<Location>(1);
                }

                results.Add(location);
            }

            return results ?? (IReadOnlyList<Location>)Array.Empty<Location>();
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(string text, SyntaxTree syntaxTree, in int spanStart, in ReadOnlySpan<string> values, in StringComparison comparison, in int startOffset, in int endOffset, in int textLength)
        {
            List<Location> alreadyReportedLocations = null;
            List<Location> results = null;

            for (int valueIndex = 0, valuesCount = values.Length; valueIndex < valuesCount; valueIndex++)
            {
                var value = values[valueIndex];

                if (textLength < value.Length)
                {
                    // nothing to inspect as the text is too short
                    continue;
                }

                var allIndices = text.AllIndicesOf(value, comparison);

                if (allIndices.Length is 0)
                {
                    // nothing to inspect
                    continue;
                }

                for (int index = 0, length = allIndices.Length; index < length; index++)
                {
                    var position = allIndices[index];

                    var location = value.GetLocation(syntaxTree, spanStart, position, startOffset, endOffset);

                    if (location is null)
                    {
                        continue;
                    }

                    if (alreadyReportedLocations is null)
                    {
                        alreadyReportedLocations = new List<Location>(1);
                    }
                    else
                    {
                        if (alreadyReportedLocations.Exists(location.IntersectsWith))
                        {
                            // already reported, so ignore it
                            continue;
                        }
                    }

                    alreadyReportedLocations.Add(location);

                    if (results is null)
                    {
                        results = new List<Location>(1);
                    }

                    results.Add(location);
                }
            }

            return results ?? (IReadOnlyList<Location>)Array.Empty<Location>();
        }

        private static IReadOnlyList<Location> GetAllLocationsWithLoop(string text, SyntaxTree syntaxTree, in int spanStart, string value, Func<char, bool> nextCharValidationCallback, in int startOffset, in int endOffset, in int textLength, in ReadOnlySpan<int> allIndices)
        {
            var lastPosition = textLength - 1;

            List<Location> alreadyReportedLocations = null;
            List<Location> results = null;

            for (int index = 0, length = allIndices.Length; index < length; index++)
            {
                var position = allIndices[index];
                var afterPosition = position + value.Length;

                if (afterPosition <= lastPosition)
                {
                    if (nextCharValidationCallback(text[afterPosition]) is false)
                    {
                        continue;
                    }
                }

                var location = value.GetLocation(syntaxTree, spanStart, position, startOffset, endOffset);

                if (location is null)
                {
                    continue;
                }

                if (alreadyReportedLocations is null)
                {
                    alreadyReportedLocations = new List<Location>(1);
                }
                else
                {
                    if (alreadyReportedLocations.Exists(location.IntersectsWith))
                    {
                        // already reported, so ignore it
                        continue;
                    }
                }

                alreadyReportedLocations.Add(location);

                if (results is null)
                {
                    results = new List<Location>(1);
                }

                results.Add(location);
            }

            return results ?? (IReadOnlyList<Location>)Array.Empty<Location>();
        }
    }
}
