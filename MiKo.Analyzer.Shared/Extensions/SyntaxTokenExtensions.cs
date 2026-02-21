using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxToken"/>s.
    /// </summary>
    internal static class SyntaxTokenExtensions
    {
        /// <summary>
        /// Gets all ancestors of the specified kind for the syntax token.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the ancestor to seek.
        /// </typeparam>
        /// <param name="value">
        /// The syntax token to get the ancestors for.
        /// </param>
        /// <returns>
        /// A sequence that contains all ancestor nodes of the specified type.
        /// </returns>
        internal static IEnumerable<T> Ancestors<T>(this in SyntaxToken value) where T : SyntaxNode => value.Parent.Ancestors<T>();

        /// <summary>
        /// Creates a syntax token from the specified syntax kind.
        /// </summary>
        /// <param name="value">
        /// One of the enumeration members that specifies the syntax kind to convert to a token.
        /// </param>
        /// <returns>
        /// A new syntax token of the specified kind.
        /// </returns>
        internal static SyntaxToken AsToken(this SyntaxKind value) => SyntaxFactory.Token(value);

        /// <summary>
        /// Gets all comments associated with the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get comments from.
        /// </param>
        /// <returns>
        /// An array of comment trivia.
        /// </returns>
        internal static SyntaxTrivia[] GetComment(this in SyntaxToken value) => value.GetAllTrivia().Where(_ => _.IsComment()).ToArray();

        /// <summary>
        /// Gets the documentation comment trivia syntax for the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the documentation comment from.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of documentation comment to get.
        /// Defaults to <see cref="SyntaxKind.SingleLineDocumentationCommentTrivia"/>.
        /// </param>
        /// <returns>
        /// An array of documentation comment trivia syntax nodes.
        /// </returns>
        internal static DocumentationCommentTriviaSyntax[] GetDocumentationCommentTriviaSyntax(this in SyntaxToken value, in SyntaxKind kind = SyntaxKind.SingleLineDocumentationCommentTrivia)
        {
            var leadingTrivia = value.LeadingTrivia;
            var count = leadingTrivia.Count;

            // Perf: quick check to avoid costly loop
            if (count >= 2)
            {
                var trivia = leadingTrivia[count is 4 ? 2 : 1];

                if (trivia.IsKind(kind) && trivia.GetStructure() is DocumentationCommentTriviaSyntax syntax)
                {
                    return new[] { syntax };
                }
            }

            DocumentationCommentTriviaSyntax[] results = null;

            for (int index = 0, resultsIndex = 0; index < count; index++)
            {
                var trivia = leadingTrivia[index];

                if (trivia.IsKind(kind) && trivia.GetStructure() is DocumentationCommentTriviaSyntax syntax)
                {
                    if (results is null)
                    {
                        results = new DocumentationCommentTriviaSyntax[1];
                    }
                    else
                    {
                        // seems we have more separate comments, so increase by one
                        Array.Resize(ref results, results.Length + 1);
                    }

                    results[resultsIndex] = syntax;

                    resultsIndex++;
                }
            }

            return results ?? Array.Empty<DocumentationCommentTriviaSyntax>();
        }

        /// <summary>
        /// Gets the first enclosing node of the specified type containing the syntax token.
        /// </summary>
        /// <typeparam name="T">
        /// The type of node to get.
        /// </typeparam>
        /// <param name="value">
        /// The syntax token to get the enclosing node for.
        /// </param>
        /// <returns>
        /// The enclosing node of the specified type.
        /// </returns>
        internal static T GetEnclosing<T>(this in SyntaxToken value) where T : SyntaxNode => value.Parent.GetEnclosing<T>();

        /// <summary>
        /// Gets the ending line number of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the line for.
        /// </param>
        /// <returns>
        /// The line number where the token ends.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetEndingLine(this in SyntaxToken value) => value.GetLocation().GetEndingLine();

        /// <summary>
        /// Gets the end position of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the position for.
        /// </param>
        /// <returns>
        /// The line position at the end of the token.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetEndPosition(this in SyntaxToken value) => value.GetLocation().GetEndPosition();

        /// <summary>
        /// Gets the line span for the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the line span for.
        /// </param>
        /// <returns>
        /// The file line position span of the token.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FileLinePositionSpan GetLineSpan(this in SyntaxToken value) => value.GetLocation().GetLineSpan();

        /// <summary>
        /// Gets the position after the end of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the position for.
        /// </param>
        /// <returns>
        /// The line position after the end of the token.
        /// </returns>
        internal static LinePosition GetPositionAfterEnd(this in SyntaxToken value)
        {
            var position = value.GetEndPosition();

            return new LinePosition(position.Line, position.Character + 1);
        }

        /// <summary>
        /// Gets the position within the end line of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the position for.
        /// </param>
        /// <returns>
        /// The position within the end line of the token.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinEndLine(this in SyntaxToken value) => value.GetLocation().GetPositionWithinEndLine();

        /// <summary>
        /// Gets the position within the start line of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the position for.
        /// </param>
        /// <returns>
        /// The position within the start line of the token.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinStartLine(this in SyntaxToken value) => value.GetLocation().GetPositionWithinStartLine();

        /// <summary>
        /// Gets the starting line number of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the line for.
        /// </param>
        /// <returns>
        /// The line number where the token starts.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetStartingLine(this in SyntaxToken value) => value.GetLocation().GetStartingLine();

        /// <summary>
        /// Gets the start position of the syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the position for.
        /// </param>
        /// <returns>
        /// The line position at the start of the token.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetStartPosition(this in SyntaxToken value) => value.GetLocation().GetStartPosition();

        /// <summary>
        /// Gets the symbol associated with the syntax token from the specified compilation.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the symbol for.
        /// </param>
        /// <param name="compilation">
        /// The compilation to use for semantic analysis.
        /// </param>
        /// <returns>
        /// The symbol associated with the token, or <see langword="null"/> if no symbol is found.
        /// </returns>
        internal static ISymbol GetSymbol(this in SyntaxToken value, Compilation compilation)
        {
            var syntaxTree = value.SyntaxTree;

            if (syntaxTree is null)
            {
                return null;
            }

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            return value.GetSymbol(semanticModel);
        }

        /// <summary>
        /// Gets the symbol associated with the syntax token from the specified semantic model.
        /// </summary>
        /// <param name="value">
        /// The syntax token to get the symbol for.
        /// </param>
        /// <param name="semanticModel">
        /// The semantic model to use for symbol lookup.
        /// </param>
        /// <returns>
        /// The symbol associated with the token, or <see langword="null"/> if no symbol is found.
        /// </returns>
        internal static ISymbol GetSymbol(this in SyntaxToken value, SemanticModel semanticModel)
        {
            var syntaxNode = value.Parent;

            // try to find the node as that may be faster than to look them up
            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);

            if (symbol is null)
            {
                var position = value.GetLocation().SourceSpan.Start;
                var name = value.ValueText;
                var symbols = semanticModel.LookupSymbols(position, name: name);

                if (symbols.Length > 0)
                {
                    return symbols[0];
                }
            }

            return symbol;
        }

        /// <summary>
        /// Determines whether the syntax token has any comments.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token has any comments; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasComment(this in SyntaxToken value) => value.HasLeadingComment() || value.HasTrailingComment();

        /// <summary>
        /// Determines whether the syntax token has a documentation comment trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token has a documentation comment trivia; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasDocumentationCommentTriviaSyntax(this in SyntaxToken value)
        {
            var leadingTrivia = value.LeadingTrivia;
            var count = leadingTrivia.Count;

            // Perf: quick check to avoid costly loop
            if (count >= 2)
            {
                if (leadingTrivia[count is 4 ? 2 : 1].IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return true;
                }
            }

            for (var index = 0; index < count; index++)
            {
                if (leadingTrivia[index].IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the syntax token has any leading comments.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token has leading comments; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool HasLeadingComment(this in SyntaxToken value) => value.LeadingTrivia.HasComment();

        /// <summary>
        /// Determines whether the syntax token has any trailing comments.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token has trailing comments; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool HasTrailingComment(this in SyntaxToken value) => value.TrailingTrivia.HasComment();

        /// <summary>
        /// Determines whether the syntax token has a trailing end of line.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token has a trailing end of line; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool HasTrailingEndOfLine(this in SyntaxToken value) => value.TrailingTrivia.HasEndOfLine();

        /// <summary>
        /// Determines whether the syntax token is any of the specified kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="kinds">
        /// The set of syntax kinds to check against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is any of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsAnyKind(this in SyntaxToken value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        /// <summary>
        /// Determines whether the syntax token is any of the specified kinds.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="kinds">
        /// The span of syntax kinds to check against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is any of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAnyKind(this in SyntaxToken value, in ReadOnlySpan<SyntaxKind> kinds)
        {
            var valueKind = value.Kind();

            for (int index = 0, length = kinds.Length; index < length; index++)
            {
                if (kinds[index] == valueKind)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the syntax token is the default value.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is the default value; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDefaultValue(this in SyntaxToken value) => value.IsKind(SyntaxKind.None);

        /// <summary>
        /// Determines whether the token is of the specified kind.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is of the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsKind(this in SyntaxToken value, in SyntaxKind kind) => value.RawKind == (int)kind;

        /// <summary>
        /// Determines whether the syntax token is located at the specified location.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="location">
        /// The location to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is at the specified location; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsLocatedAt(this in SyntaxToken value, Location location) => value.GetLocation().Equals(location);

        /// <summary>
        /// Determines whether the syntax token is on the same line as the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="other">
        /// The syntax node to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is on the same line as the node; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAs(this in SyntaxToken value, SyntaxNode other) => value.GetStartingLine() == other?.GetStartingLine();

        /// <summary>
        /// Determines whether the syntax token is on the same line as the specified syntax node or token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="other">
        /// The syntax node or token to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is on the same line as the node or token; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAs(this in SyntaxToken value, in SyntaxNodeOrToken other) => value.GetStartingLine() == other.GetStartingLine();

        /// <summary>
        /// Determines whether the syntax token is on the same line as another syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="other">
        /// The other syntax token to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is on the same line as the other token; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAs(this in SyntaxToken value, in SyntaxToken other) => value.GetStartingLine() == other.GetStartingLine();

        /// <summary>
        /// Determines whether the syntax token is on the same line as the end of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="other">
        /// The syntax node to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is on the same line as the end of the node; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAsEndOf(this in SyntaxToken value, SyntaxNode other) => value.GetStartingLine() == other?.GetEndingLine();

        /// <summary>
        /// Determines whether the syntax token is on the same line as the end of the specified syntax node or token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="other">
        /// The syntax node or token to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is on the same line as the end of the node or token; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAsEndOf(this in SyntaxToken value, in SyntaxNodeOrToken other) => value.GetStartingLine() == other.GetEndingLine();

        /// <summary>
        /// Determines whether the syntax token is on the same line as the end of another syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <param name="other">
        /// The other syntax token to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token is on the same line as the end of the other token; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAsEndOf(this in SyntaxToken value, in SyntaxToken other) => value.GetStartingLine() == other.GetEndingLine();

        /// <summary>
        /// Determines whether the syntax token spans multiple lines.
        /// </summary>
        /// <param name="value">
        /// The syntax token to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the token spans multiple lines; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSpanningMultipleLines(this in SyntaxToken value)
        {
            var leadingTrivia = value.LeadingTrivia;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var count = leadingTrivia.Count;

            if (count > 0)
            {
                var foundLine = false;

                for (var index = 0; index < count; index++)
                {
                    if (leadingTrivia[index].IsComment())
                    {
                        if (foundLine)
                        {
                            return true;
                        }

                        foundLine = true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Filters the source tokens for those of the specified kind.
        /// </summary>
        /// <param name="source">
        /// The source collection of tokens.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to filter by.
        /// </param>
        /// <returns>
        /// A sequence that contains only tokens of the specified kind.
        /// </returns>
        internal static IEnumerable<SyntaxToken> OfKind(this IEnumerable<SyntaxToken> source, SyntaxKind kind)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in source)
            {
                if (item.IsKind(kind))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Converts a collection of syntax kinds to a token list.
        /// </summary>
        /// <param name="source">
        /// The source collection of syntax kinds.
        /// </param>
        /// <returns>
        /// A collection of a syntax tokens for each of the specified kinds.
        /// </returns>
        internal static SyntaxTokenList ToTokenList(this IEnumerable<SyntaxKind> source) => source.Select(_ => _.AsToken()).ToTokenList();

        /// <summary>
        /// Creates a new syntax token with additional spaces as leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="additionalSpaces">
        /// The number of additional spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax token with additional spaces as leading trivia.
        /// </returns>
        internal static SyntaxToken WithAdditionalLeadingSpaces(this in SyntaxToken value, in int additionalSpaces)
        {
            var currentSpaces = value.GetPositionWithinStartLine();

            // the spaces might get negative, so ensure that they are at least zero
            var spaces = Math.Max(0, currentSpaces + additionalSpaces);

            return value.WithLeadingSpaces(spaces);
        }

        /// <summary>
        /// Creates a new syntax token with additional spaces at the end of its leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="additionalSpaces">
        /// The number of additional spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax token with additional spaces at the end of its leading trivia.
        /// </returns>
        internal static SyntaxToken WithAdditionalLeadingSpacesAtEnd(this in SyntaxToken value, in int additionalSpaces)
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            return value.WithAdditionalLeadingTrivia(WhiteSpaces(additionalSpaces));
        }

        /// <summary>
        /// Creates a new syntax token with additional leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="trivia">
        /// The trivia to add.
        /// </param>
        /// <returns>
        /// A new syntax token with the additional leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithAdditionalLeadingTrivia(this in SyntaxToken value, in SyntaxTrivia trivia) => value.WithLeadingTrivia(value.LeadingTrivia.Add(trivia));

        /// <summary>
        /// Creates a new syntax token with additional leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="trivia">
        /// The trivia list to add.
        /// </param>
        /// <returns>
        /// A new syntax token with the additional leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithAdditionalLeadingTrivia(this in SyntaxToken value, in SyntaxTriviaList trivia) => value.WithLeadingTrivia(value.LeadingTrivia.AddRange(trivia));

        /// <summary>
        /// Creates a new syntax token with additional leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="trivia">
        /// The collection of trivia to add.
        /// </param>
        /// <returns>
        /// A new syntax token with the additional leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithAdditionalLeadingTrivia(this in SyntaxToken value, IEnumerable<SyntaxTrivia> trivia) => value.WithLeadingTrivia(value.LeadingTrivia.AddRange(trivia));

        /// <summary>
        /// Creates a new syntax token with additional leading trivia from a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="node">
        /// The node to get the leading trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with the additional leading trivia.
        /// </returns>
        internal static SyntaxToken WithAdditionalLeadingTriviaFrom(this in SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new syntax token with additional leading trivia from another token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="token">
        /// The token to get the leading trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with the additional leading trivia.
        /// </returns>
        internal static SyntaxToken WithAdditionalLeadingTriviaFrom(this in SyntaxToken value, in SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new syntax token with an end of line added as trailing trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with an end of line added as trailing trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithEndOfLine(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        /// <summary>
        /// Creates a new syntax token with a space as both leading and trailing trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a space as leading and trailing trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithLeadingAndTrailingSpace(this in SyntaxToken value) => value.WithLeadingSpace().WithTrailingSpace();

        /// <summary>
        /// Creates a new syntax token with a leading empty line.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a leading empty line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithLeadingEmptyLine(this in SyntaxToken value) => value.WithLeadingEmptyLines(1);

        /// <summary>
        /// Creates a new syntax token with the specified number of leading empty lines.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="lines">
        /// The number of empty lines to add.
        /// </param>
        /// <returns>
        /// A new syntax token with the specified number of leading empty lines.
        /// </returns>
        internal static SyntaxToken WithLeadingEmptyLines(this in SyntaxToken value, in int lines)
        {
            var trivia = value.LeadingTrivia;

            // remove existing empty lines
            var emptyLineIndices = new Stack<int>();

            for (int index = 0, count = trivia.Count; index < count; index++)
            {
                if (trivia[index].IsEndOfLine())
                {
                    emptyLineIndices.Push(index);
                }
            }

            foreach (var index in emptyLineIndices)
            {
                trivia = trivia.RemoveAt(index);
            }

            // add new lines
            for (var i = 0; i < lines; i++)
            {
                trivia = trivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again
            }

            return value.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Creates a new syntax token with an end of line added as leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with an end of line added as leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithLeadingEndOfLine(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again

        /// <summary>
        /// Creates a new syntax token with a single space as leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a space as leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithLeadingSpace(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.Space); // use non-elastic one to prevent formatting to be done automatically

        /// <summary>
        /// Creates a new syntax token with the specified number of spaces as leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="count">
        /// The number of spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax token with the specified number of spaces as leading trivia.
        /// </returns>
        internal static SyntaxToken WithLeadingSpaces(this in SyntaxToken value, in int count)
        {
            if (value.HasLeadingComment())
            {
                // we have to add the spaces after the comment, so we have to determine the additional spaces to add
                var additionalSpaces = count - value.GetPositionWithinStartLine();

                var leadingTrivia = value.LeadingTrivia.ToList();

                // first collect all indices of the comments, for later reference
                var commentIndices = new List<int>(1);

                for (int index = 0, triviaCount = leadingTrivia.Count; index < triviaCount; index++)
                {
                    var trivia = leadingTrivia[index];

                    if (trivia.IsComment())
                    {
                        commentIndices.Add(index);
                    }
                }

                // ensure proper size to avoid multiple resizes
                leadingTrivia.Capacity += commentIndices.Count;

                // now update the comments, but adjust the offset to remember the already added spaces (trivia indices change due to that)
                var offset = 0;

                foreach (var index in commentIndices)
                {
                    leadingTrivia.Insert(index + offset, WhiteSpaces(additionalSpaces));

                    offset += 1;
                }

                return value.WithLeadingTrivia(leadingTrivia)
                            .WithAdditionalLeadingTrivia(WhiteSpaces(additionalSpaces));
            }

            return value.WithLeadingTrivia(WhiteSpaces(count));
        }

        /// <summary>
        /// Creates a new syntax token with leading trivia from a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="node">
        /// The node to get the leading trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with leading trivia from the specified node.
        /// </returns>
        internal static SyntaxToken WithLeadingTriviaFrom(this in SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new syntax token with leading trivia from another token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="token">
        /// The token to get the leading trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with leading trivia from the specified token.
        /// </returns>
        internal static SyntaxToken WithLeadingTriviaFrom(this in SyntaxToken value, in SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new syntax token with an XML comment start as leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with an XML comment start as leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithLeadingXmlComment(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        /// <summary>
        /// Creates a new syntax token with an XML comment exterior as leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with an XML comment exterior as leading trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithLeadingXmlCommentExterior(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentExterior);

        /// <summary>
        /// Creates a new syntax token without leading trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token without leading trivia but preserving trailing trivia.
        /// </returns>
        internal static SyntaxToken WithoutLeadingTrivia(this in SyntaxToken value) => value.WithoutTrivia().WithTrailingTrivia(value.TrailingTrivia);

        /// <summary>
        /// Creates a new syntax token without trailing trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token without trailing trivia but preserving leading trivia.
        /// </returns>
        internal static SyntaxToken WithoutTrailingTrivia(this in SyntaxToken value) => value.WithoutTrivia().WithLeadingTrivia(value.LeadingTrivia);

        /// <summary>
        /// Creates a new syntax token with a space both before and after.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a space as leading and trailing trivia.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithSurroundingSpace(this in SyntaxToken value) => value.WithLeadingSpace().WithTrailingSpace();

        /// <summary>
        /// Creates a new syntax token with the specified text.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="text">
        /// The new text for the token.
        /// </param>
        /// <returns>
        /// A new syntax token with the specified text.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithText(this in SyntaxToken value, string text) => SyntaxFactory.Token(value.LeadingTrivia, value.Kind(), text, text, value.TrailingTrivia);

        /// <summary>
        /// Creates a new syntax token with the specified text.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="text">
        /// The new text for the token.
        /// </param>
        /// <returns>
        /// A new syntax token with the specified text.
        /// </returns>
        internal static SyntaxToken WithText(this in SyntaxToken value, in ReadOnlySpan<char> text) => value.WithText(text.ToString());

        /// <summary>
        /// Creates a new syntax token with a trailing empty line.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a trailing empty line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithTrailingEmptyLine(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        /// <summary>
        /// Creates a new syntax token with a trailing new line.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a trailing new line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithTrailingNewLine(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        /// <summary>
        /// Creates a new syntax token with a trailing space.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a trailing space.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithTrailingSpace(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.Space); // use non-elastic one to prevent formatting to be done automatically

        /// <summary>
        /// Creates a new syntax token with trailing trivia from a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="node">
        /// The node to get the trailing trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with trailing trivia from the specified node.
        /// </returns>
        internal static SyntaxToken WithTrailingTriviaFrom(this in SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetTrailingTrivia();

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new syntax token with trailing trivia from another token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="token">
        /// The token to get the trailing trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with trailing trivia from the specified token.
        /// </returns>
        internal static SyntaxToken WithTrailingTriviaFrom(this in SyntaxToken value, in SyntaxToken token)
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new syntax token with a trailing XML comment.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <returns>
        /// A new syntax token with a trailing XML comment.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxToken WithTrailingXmlComment(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        /// <summary>
        /// Creates a new syntax token with trivia from the specified node.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="node">
        /// The node to get the trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with leading and trailing trivia from the specified node.
        /// </returns>
        internal static SyntaxToken WithTriviaFrom(this in SyntaxToken value, SyntaxNode node) => value.WithLeadingTriviaFrom(node)
                                                                                                       .WithTrailingTriviaFrom(node);

        /// <summary>
        /// Creates a new syntax token with trivia from another token.
        /// </summary>
        /// <param name="value">
        /// The syntax token to modify.
        /// </param>
        /// <param name="token">
        /// The token to get the trivia from.
        /// </param>
        /// <returns>
        /// A new syntax token with leading and trailing trivia from the specified token.
        /// </returns>
        internal static SyntaxToken WithTriviaFrom(this in SyntaxToken value, in SyntaxToken token) => value.WithLeadingTriviaFrom(token)
                                                                                                            .WithTrailingTriviaFrom(token);

        /// <summary>
        /// Creates a new syntax token with the specified number of white spaces.
        /// </summary>
        /// <param name="count">
        /// The number of white space characters to include.
        /// </param>
        /// <returns>
        /// A syntax trivia representing the specified number of white spaces.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SyntaxTrivia WhiteSpaces(in int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}