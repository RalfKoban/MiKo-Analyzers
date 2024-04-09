using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxTokenExtensions
    {
        internal static IEnumerable<T> Ancestors<T>(this SyntaxToken value) where T : SyntaxNode => value.Parent.Ancestors<T>();

        internal static SyntaxToken AsToken(this SyntaxKind value) => SyntaxFactory.Token(value);

        internal static IEnumerable<SyntaxToken> DescendantTokens(this SyntaxNode value, SyntaxKind kind) => value.DescendantTokens().OfKind(kind);

        internal static SyntaxToken First(this SyntaxTokenList value, SyntaxKind kind) => value.OfKind(kind).FirstOrDefault();

        internal static T GetEnclosing<T>(this SyntaxToken value) where T : SyntaxNode => value.Parent.GetEnclosing<T>();

        internal static int GetPositionWithinStartLine(this SyntaxToken value) => value.GetLocation().GetPositionWithinStartLine();

        internal static LinePosition GetStartPosition(this SyntaxToken value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxToken value) => value.GetLocation().GetEndPosition();

        internal static int GetStartingLine(this SyntaxToken value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxToken value) => value.GetLocation().GetEndingLine();

        internal static ISymbol GetSymbol(this SyntaxToken value, Compilation compilation)
        {
            var syntaxTree = value.SyntaxTree;

            if (syntaxTree is null)
            {
                return null;
            }

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            return value.GetSymbol(semanticModel);
        }

        internal static ISymbol GetSymbol(this SyntaxToken value, SemanticModel semanticModel)
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

        internal static SyntaxTrivia[] GetComment(this SyntaxToken value) => value.GetAllTrivia().Where(_ => _.IsComment()).ToArray();

        internal static bool HasComment(this SyntaxToken value) => value.GetAllTrivia().Any(_ => _.IsComment());

        internal static bool HasLeadingComment(this SyntaxToken value) => value.LeadingTrivia.Any(_ => _.IsComment());

        internal static bool HasTrailingComment(this SyntaxToken value) => value.TrailingTrivia.Any(_ => _.IsComment());

        internal static bool IsDefaultValue(this SyntaxToken value) => value.IsKind(SyntaxKind.None);

        internal static bool IsAnyKind(this SyntaxToken value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        internal static bool IsAnyKind(this SyntaxToken value, params SyntaxKind[] kinds)
        {
            var valueKind = value.Kind();

            // ReSharper disable once LoopCanBeConvertedToQuery  : For performance reasons we use indexing instead of an enumerator
            // ReSharper disable once ForCanBeConvertedToForeach : For performance reasons we use indexing instead of an enumerator
            for (var index = 0; index < kinds.Length; index++)
            {
                if (kinds[index] == valueKind)
                {
                    return true;
                }
            }

            return false;
        }

        internal static IReadOnlyList<SyntaxToken> OfKind(this SyntaxTokenList source, SyntaxKind kind)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount == 0)
            {
                return Array.Empty<SyntaxToken>();
            }

            var results = new List<SyntaxToken>();

            for (var index = 0; index < sourceCount; index++)
            {
                var item = source[index];

                if (item.IsKind(kind))
                {
                    results.Add(item);
                }
            }

            return results;
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

        internal static SyntaxToken WithEndOfLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingEmptyLine(this SyntaxToken value) => value.WithLeadingTrivia(value.LeadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed));

        internal static SyntaxToken WithLeadingEndOfLine(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again

        internal static SyntaxToken WithLeadingSpace(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithAdditionalLeadingSpaces(this SyntaxToken value, int additionalSpaces)
        {
            var currentSpaces = value.GetPositionWithinStartLine();

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static SyntaxToken WithAdditionalLeadingTrivia(this SyntaxToken value, SyntaxTriviaList trivia) => value.WithLeadingTrivia(value.LeadingTrivia.AddRange(trivia));

        internal static SyntaxToken WithAdditionalLeadingTrivia(this SyntaxToken value, params SyntaxTrivia[] trivia) => value.WithLeadingTrivia(value.LeadingTrivia.AddRange(trivia));

        internal static SyntaxToken WithAdditionalLeadingTriviaFrom(this SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithAdditionalLeadingTriviaFrom(this SyntaxToken value, SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithLeadingSpaces(this SyntaxToken value, int count) => value.WithLeadingTrivia(Enumerable.Repeat(SyntaxFactory.Space, count)); // use non-elastic one to prevent formatting to be done automatically

        internal static SyntaxToken WithLeadingXmlComment(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        internal static SyntaxToken WithTriviaFrom(this SyntaxToken value, SyntaxNode node) => value.WithLeadingTriviaFrom(node)
                                                                                                    .WithTrailingTriviaFrom(node);

        internal static SyntaxToken WithTriviaFrom(this SyntaxToken value, SyntaxToken token) => value.WithLeadingTriviaFrom(token)
                                                                                                      .WithTrailingTriviaFrom(token);

        internal static SyntaxToken WithLeadingTriviaFrom(this SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithLeadingTriviaFrom(this SyntaxToken value, SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithTrailingTriviaFrom(this SyntaxToken value, SyntaxNode node)
        {
            var trivia = node.GetTrailingTrivia();

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static SyntaxToken WithTrailingTriviaFrom(this SyntaxToken value, SyntaxToken token)
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static SyntaxTokenList WithoutFirstXmlNewLine(this SyntaxTokenList values)
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

        internal static SyntaxTokenList WithoutLastXmlNewLine(this SyntaxTokenList values)
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

        internal static SyntaxToken WithoutLeadingTrivia(this SyntaxToken value) => value.WithoutTrivia().WithTrailingTrivia(value.TrailingTrivia);

        internal static SyntaxToken WithoutTrailingTrivia(this SyntaxToken value) => value.WithoutTrivia().WithLeadingTrivia(value.LeadingTrivia);

        internal static SyntaxTokenList WithoutEmptyText(this SyntaxTokenList values, SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.ValueText.IsNullOrWhiteSpace())
            {
                return values.Remove(token);
            }

            return values;
        }

        internal static SyntaxTokenList WithoutNewLine(this SyntaxTokenList values, SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
            {
                return values.Remove(token);
            }

            return values;
        }

        internal static SyntaxToken WithSurroundingSpace(this SyntaxToken value) => value.WithLeadingSpace().WithTrailingSpace();

        internal static SyntaxToken WithText(this SyntaxToken value, string text) => SyntaxFactory.Token(value.LeadingTrivia, value.Kind(), text, text, value.TrailingTrivia);

        internal static SyntaxToken WithText(this SyntaxToken value, StringBuilder text) => WithText(value, text.ToString());

        internal static SyntaxToken WithText(this SyntaxToken value, ReadOnlySpan<char> text) => value.WithText(text.ToString());

        internal static SyntaxToken WithTrailingEmptyLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingNewLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingSpace(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.Space); // use non-elastic one to prevent formatting to be done automatically

        internal static SyntaxToken WithTrailingXmlComment(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxNodeExtensions.XmlCommentStart);
    }
}