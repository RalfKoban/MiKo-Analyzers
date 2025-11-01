using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNodeOrToken"/> instances to retrieve position and line information.
    /// </summary>
    internal static class SyntaxNodeOrTokenExtensions
    {
        /// <summary>
        /// Gets the position within the starting line where the syntax node or token begins.
        /// </summary>
        /// <param name="value">
        /// The syntax node or token to get the position for.
        /// </param>
        /// <returns>
        /// The zero-based character position within the starting line.
        /// </returns>
        internal static int GetPositionWithinStartLine(this in SyntaxNodeOrToken value) => value.GetLocation().GetPositionWithinStartLine();

        /// <summary>
        /// Gets the line number where the syntax node or token starts.
        /// </summary>
        /// <param name="value">
        /// The syntax node or token to get the starting line for.
        /// </param>
        /// <returns>
        /// The zero-based line number where the syntax node or token begins.
        /// </returns>
        internal static int GetStartingLine(this in SyntaxNodeOrToken value) => value.GetLocation().GetStartingLine();

        /// <summary>
        /// Gets the line number where the syntax node or token ends.
        /// </summary>
        /// <param name="value">
        /// The syntax node or token to get the ending line for.
        /// </param>
        /// <returns>
        /// The zero-based line number where the syntax node or token ends.
        /// </returns>
        internal static int GetEndingLine(this in SyntaxNodeOrToken value) => value.GetLocation().GetEndingLine();

        /// <summary>
        /// Gets the starting position of the syntax node or token.
        /// </summary>
        /// <param name="value">
        /// The syntax node or token to get the starting position for.
        /// </param>
        /// <returns>
        /// The line position where the syntax node or token begins.
        /// </returns>
        internal static LinePosition GetStartPosition(this in SyntaxNodeOrToken value) => value.GetLocation().GetStartPosition();

        /// <summary>
        /// Gets the ending position of the syntax node or token.
        /// </summary>
        /// <param name="value">
        /// The syntax node or token to get the ending position for.
        /// </param>
        /// <returns>
        /// The line position where the syntax node or token ends.
        /// </returns>
        internal static LinePosition GetEndPosition(this in SyntaxNodeOrToken value) => value.GetLocation().GetEndPosition();
    }
}