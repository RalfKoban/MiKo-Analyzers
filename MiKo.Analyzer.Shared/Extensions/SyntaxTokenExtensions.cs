using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxTokenExtensions
    {
        internal static IEnumerable<T> Ancestors<T>(this SyntaxToken value) where T : SyntaxNode => value.Parent.Ancestors<T>();

        internal static IEnumerable<SyntaxToken> DescendantTokens(this SyntaxNode value, SyntaxKind kind) => value.DescendantTokens().OfKind(kind);

        internal static T GetEnclosing<T>(this SyntaxToken value) where T : SyntaxNode => value.Parent.GetEnclosing<T>();

        internal static LinePosition GetStartPosition(this SyntaxToken value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxToken value) => value.GetLocation().GetEndPosition();

        internal static int GetStartingLine(this SyntaxToken value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxToken value) => value.GetLocation().GetEndingLine();

        internal static ISymbol GetSymbol(this SyntaxToken value, Compilation compilation)
        {
            if (value.SyntaxTree is null)
            {
                return null;
            }

            var semanticModel = compilation.GetSemanticModel(value.SyntaxTree);

            return value.GetSymbol(semanticModel);
        }

        internal static ISymbol GetSymbol(this SyntaxToken value, SemanticModel semanticModel)
        {
            var position = value.GetLocation().SourceSpan.Start;
            var name = value.ValueText;
            var syntaxNode = value.Parent;

            if (syntaxNode is ParameterSyntax node)
            {
                // we might have a ctor here and no method
                var methodName = node.GetMethodName();
                var methodSymbols = semanticModel.LookupSymbols(position, name: methodName).OfType<IMethodSymbol>();
                var parameterSymbol = methodSymbols.SelectMany(_ => _.Parameters).FirstOrDefault(_ => _.Name == name);

                return parameterSymbol;

                // if it's no method parameter, then it is a local one (but Roslyn cannot handle that currently in v3.3)
                // var symbol = semanticModel.LookupSymbols(position).First(_ => _.Kind == SymbolKind.Local);
            }

            // try to find the node as that may be faster than to look them up
            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);

            if (symbol is null)
            {
                var symbols = semanticModel.LookupSymbols(position, name: name);

                if (symbols.Length > 0)
                {
                    return symbols[0];
                }
            }

            return symbol;
        }

        internal static bool HasTrailingComment(this SyntaxToken value) => value.TrailingTrivia.Any(_ => _.IsComment());

        internal static bool IsDefaultValue(this SyntaxToken value) => value.IsKind(SyntaxKind.None);

        internal static IEnumerable<SyntaxToken> OfKind(this SyntaxTokenList source, SyntaxKind kind)
        {
            if (source.Count == 0)
            {
                return Array.Empty<SyntaxToken>();
            }

            var results = new List<SyntaxToken>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < source.Count; index++)
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

        internal static SyntaxToken ToSyntaxToken(this string source, SyntaxKind kind = SyntaxKind.StringLiteralToken)
        {
            switch (kind)
            {
                case SyntaxKind.IdentifierToken:
                    return SyntaxFactory.Identifier(source);

                default:
                    return SyntaxFactory.Token(default, kind, source, source, default);
            }
        }

        internal static SyntaxToken WithEndOfLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingEmptyLine(this SyntaxToken value) => value.WithLeadingTrivia(value.LeadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed));

        internal static SyntaxToken WithLeadingEndOfLine(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingSpace(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithAdditionalLeadingSpaces(this SyntaxToken value, int additionalSpaces)
        {
            var currentSpaces = value.GetStartPosition().Character;

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static SyntaxToken WithLeadingSpaces(this SyntaxToken value, int count) => value.WithLeadingTrivia(Enumerable.Repeat(SyntaxFactory.Space, count)); // use non-elastic one to prevent formatting to be done automatically

        internal static SyntaxToken WithLeadingXmlComment(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        internal static SyntaxToken WithTriviaFrom(this SyntaxToken value, SyntaxNode node) => value.WithLeadingTriviaFrom(node)
                                                                                                    .WithTrailingTriviaFrom(node);

        internal static SyntaxToken WithTriviaFrom(this SyntaxToken value, SyntaxToken token) => value.WithLeadingTriviaFrom(token)
                                                                                                      .WithTrailingTriviaFrom(token);

        internal static SyntaxToken WithLeadingTriviaFrom(this SyntaxToken value, SyntaxNode node) => node.HasLeadingTrivia
                                                                                                      ? value.WithLeadingTrivia(node.GetLeadingTrivia())
                                                                                                      : value;

        internal static SyntaxToken WithLeadingTriviaFrom(this SyntaxToken value, SyntaxToken token) => token.HasLeadingTrivia
                                                                                                        ? value.WithLeadingTrivia(token.LeadingTrivia)
                                                                                                        : value;

        internal static SyntaxToken WithTrailingTriviaFrom(this SyntaxToken value, SyntaxNode node) => node.HasTrailingTrivia
                                                                                                       ? value.WithTrailingTrivia(node.GetTrailingTrivia())
                                                                                                       : value;

        internal static SyntaxToken WithTrailingTriviaFrom(this SyntaxToken value, SyntaxToken token) => token.HasTrailingTrivia
                                                                                                         ? value.WithTrailingTrivia(token.TrailingTrivia)
                                                                                                         : value;

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

            if (tokens.Count > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[tokens.Count - 1]);
            }

            if (tokens.Count > 0)
            {
                tokens = WithoutNewLine(tokens, tokens[tokens.Count - 1]);
            }

            return tokens;
        }

        internal static SyntaxToken WithoutLeadingTrivia(this SyntaxToken value)
        {
            return value.WithoutTrivia().WithTrailingTrivia(value.TrailingTrivia);
        }

        internal static SyntaxToken WithoutTrailingTrivia(this SyntaxToken value)
        {
            return value.WithoutTrivia().WithLeadingTrivia(value.LeadingTrivia);
        }

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