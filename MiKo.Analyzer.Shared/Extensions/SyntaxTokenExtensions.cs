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
    internal static class SyntaxTokenExtensions
    {
        internal static IEnumerable<T> Ancestors<T>(this in SyntaxToken value) where T : SyntaxNode => value.Parent.Ancestors<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsKind(this in SyntaxToken value, in SyntaxKind kind) => value.RawKind == (int)kind;

        internal static SyntaxToken AsToken(this SyntaxKind value) => SyntaxFactory.Token(value);

        internal static SyntaxToken First(this in SyntaxTokenList value, in SyntaxKind kind)
        {
            var tokens = value.OfKind(kind);

            return tokens.Count > 0 ? tokens[0] : default;
        }

        internal static T GetEnclosing<T>(this in SyntaxToken value) where T : SyntaxNode => value.Parent.GetEnclosing<T>();

        internal static int GetPositionWithinStartLine(this in SyntaxToken value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetPositionWithinEndLine(this in SyntaxToken value) => value.GetLocation().GetPositionWithinEndLine();

        internal static LinePosition GetPositionAfterEnd(this in SyntaxToken value)
        {
            var position = value.GetEndPosition();

            return new LinePosition(position.Line, position.Character + 1);
        }

        internal static LinePosition GetStartPosition(this in SyntaxToken value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this in SyntaxToken value) => value.GetLocation().GetEndPosition();

        internal static int GetStartingLine(this in SyntaxToken value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this in SyntaxToken value) => value.GetLocation().GetEndingLine();

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

        internal static SyntaxTrivia[] GetComment(this in SyntaxToken value) => value.GetAllTrivia().Where(_ => _.IsComment()).ToArray();

        internal static bool HasComment(this in SyntaxToken value) => value.HasLeadingComment() || value.HasTrailingComment();

        internal static bool HasLeadingComment(this in SyntaxToken value) => value.LeadingTrivia.HasComment();

        internal static bool HasTrailingComment(this in SyntaxToken value) => value.TrailingTrivia.HasComment();

        internal static bool HasTrailingEndOfLine(this in SyntaxToken value) => value.TrailingTrivia.HasEndOfLine();

        internal static bool IsDefaultValue(this in SyntaxToken value) => value.IsKind(SyntaxKind.None);

        internal static bool IsAnyKind(this in SyntaxToken value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

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

        internal static bool IsLocatedAt(this in SyntaxToken value, Location location) => value.GetLocation().Equals(location);

        internal static bool IsOnSameLineAs(this in SyntaxToken value, SyntaxNode other) => value.GetStartingLine() == other?.GetStartingLine();

        internal static bool IsOnSameLineAs(this in SyntaxToken value, in SyntaxNodeOrToken other) => value.GetStartingLine() == other.GetStartingLine();

        internal static bool IsOnSameLineAs(this in SyntaxToken value, in SyntaxToken other) => value.GetStartingLine() == other.GetStartingLine();

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

        internal static IReadOnlyList<SyntaxToken> OfKind(this in SyntaxTokenList source, in SyntaxKind kind)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<SyntaxToken>();
            }

            List<SyntaxToken> results = null;

            for (var index = 0; index < sourceCount; index++)
            {
                var item = source[index];

                if (item.IsKind(kind))
                {
                    if (results is null)
                    {
                        results = new List<SyntaxToken>(1);
                    }

                    results.Add(item);
                }
            }

            return results ?? (IReadOnlyList<SyntaxToken>)Array.Empty<SyntaxToken>();
        }

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

        internal static SyntaxTokenList ToTokenList(this IEnumerable<SyntaxKind> source) => source.Select(_ => _.AsToken()).ToTokenList();

        internal static SyntaxToken WithEndOfLine(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingEmptyLine(this in SyntaxToken value) => value.WithLeadingEmptyLines(1);

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

        internal static SyntaxToken WithLeadingEndOfLine(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again

        internal static SyntaxToken WithAdditionalLeadingTrivia(this in SyntaxToken value, in SyntaxTrivia trivia) => value.WithLeadingTrivia(value.LeadingTrivia.Add(trivia));

        internal static SyntaxToken WithAdditionalLeadingTrivia(this in SyntaxToken value, in SyntaxTriviaList trivia) => value.WithLeadingTrivia(value.LeadingTrivia.AddRange(trivia));

        internal static SyntaxToken WithAdditionalLeadingTrivia(this in SyntaxToken value, IEnumerable<SyntaxTrivia> trivia) => value.WithLeadingTrivia(value.LeadingTrivia.AddRange(trivia));

        internal static SyntaxToken WithAdditionalLeadingTriviaFrom(this in SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithAdditionalLeadingTriviaFrom(this in SyntaxToken value, in SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithLeadingSpace(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

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
                leadingTrivia.Capacity += additionalSpaces * commentIndices.Count;

                // now update the comments, but adjust the offset to remember the already added spaces (trivia indices change due to that)
                var offset = 0;

                foreach (var index in commentIndices)
                {
                    leadingTrivia.InsertRange(index + offset, Spaces(additionalSpaces));

                    offset += additionalSpaces;
                }

                return value.WithLeadingTrivia(leadingTrivia)
                            .WithAdditionalLeadingTrivia(Spaces(additionalSpaces));
            }

            return value.WithLeadingTrivia(Spaces(count));
        }

        internal static SyntaxToken WithAdditionalLeadingSpaces(this in SyntaxToken value, in int additionalSpaces)
        {
            var currentSpaces = value.GetPositionWithinStartLine();

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static SyntaxToken WithLeadingXmlComment(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        internal static SyntaxToken WithLeadingXmlCommentExterior(this in SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentExterior);

        internal static SyntaxToken WithTriviaFrom(this in SyntaxToken value, SyntaxNode node) => value.WithLeadingTriviaFrom(node)
                                                                                                       .WithTrailingTriviaFrom(node);

        internal static SyntaxToken WithTriviaFrom(this in SyntaxToken value, in SyntaxToken token) => value.WithLeadingTriviaFrom(token)
                                                                                                            .WithTrailingTriviaFrom(token);

        internal static SyntaxToken WithLeadingTriviaFrom(this in SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithLeadingTriviaFrom(this in SyntaxToken value, in SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithTrailingTriviaFrom(this in SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetTrailingTrivia();

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithTrailingTriviaFrom(this in SyntaxToken value, in SyntaxToken token)
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static SyntaxTokenList WithoutFirstXmlNewLine(this in SyntaxTokenList values)
        {
            var tokens = values;

            if (tokens.Count > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[0]);
            }

            if (tokens.Count > 0)
            {
                tokens = WithoutNewLine(tokens, tokens[0]);
            }

            if (tokens.Count > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[0]);
            }

            return tokens;
        }

        internal static SyntaxTokenList WithoutLastXmlNewLine(this in SyntaxTokenList values)
        {
            var tokens = values;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var tokensCount = values.Count;

            if (tokensCount > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[tokensCount - 1]);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                tokensCount = tokens.Count;
            }

            if (tokensCount > 0)
            {
                tokens = WithoutNewLine(tokens, tokens[tokensCount - 1]);
            }

            return tokens;
        }

        internal static SyntaxToken WithoutLeadingTrivia(this in SyntaxToken value) => value.WithoutTrivia().WithTrailingTrivia(value.TrailingTrivia);

        internal static SyntaxToken WithoutTrailingTrivia(this in SyntaxToken value) => value.WithoutTrivia().WithLeadingTrivia(value.LeadingTrivia);

        internal static SyntaxTokenList WithoutEmptyText(this in SyntaxTokenList values, in SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.ValueText.IsNullOrWhiteSpace())
            {
                return values.Remove(token);
            }

            return values;
        }

        internal static SyntaxTokenList WithoutNewLine(this in SyntaxTokenList values, in SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
            {
                return values.Remove(token);
            }

            return values;
        }

        internal static SyntaxToken WithSurroundingSpace(this in SyntaxToken value) => value.WithLeadingSpace().WithTrailingSpace();

        internal static SyntaxToken WithText(this in SyntaxToken value, string text) => SyntaxFactory.Token(value.LeadingTrivia, value.Kind(), text, text, value.TrailingTrivia);

        internal static SyntaxToken WithText(this in SyntaxToken value, in ReadOnlySpan<char> text) => value.WithText(text.ToString());

        internal static SyntaxToken WithTrailingEmptyLine(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingNewLine(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingSpace(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.Space); // use non-elastic one to prevent formatting to be done automatically

        internal static SyntaxToken WithTrailingXmlComment(this in SyntaxToken value) => value.WithTrailingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        private static IEnumerable<SyntaxTrivia> Spaces(in int count) => Enumerable.Repeat(SyntaxFactory.Space, count); // use non-elastic one to prevent formatting to be done automatically
    }
}