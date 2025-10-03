using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
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

            var sourceText = tree.GetText();
            var text = sourceText.ToString(TextSpan.FromBounds(0, value.SourceSpan.End));

            var lastIndexOfFirstSpace = text.LastIndexOfAny(Constants.WhiteSpaceCharacters);

            if (lastIndexOfFirstSpace is -1)
            {
                return null;
            }

            var followUpText = sourceText.GetSubText(value.SourceSpan.End).ToString();

            var firstIndexOfNextSpace = followUpText.StartsWith('<') // seems like the comment finished
                                        ? 0
                                        : followUpText.IndexOfAny(Constants.WhiteSpaceCharacters);

            if (firstIndexOfNextSpace is -1)
            {
                return null;
            }

            return sourceText.ToString(TextSpan.FromBounds(lastIndexOfFirstSpace + 1, text.Length + firstIndexOfNextSpace));
        }

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
    }
}
