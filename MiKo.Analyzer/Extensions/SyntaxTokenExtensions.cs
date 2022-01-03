using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxTokenExtensions
    {
        internal static T GetEnclosing<T>(this SyntaxToken value) where T : SyntaxNode => value.Parent.GetEnclosing<T>();

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

        internal static SyntaxToken WithEndOfLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingEmptyLine(this SyntaxToken value) => value.WithLeadingTrivia(value.LeadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed));

        internal static SyntaxToken WithLeadingEndOfLine(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingXmlComment(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        internal static SyntaxTokenList WithoutFirstXmlNewLine(this SyntaxTokenList textTokens)
        {
            var tokens = textTokens;

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

        internal static SyntaxTokenList WithoutLastXmlNewLine(this SyntaxTokenList textTokens)
        {
            var tokens = textTokens;

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

        internal static SyntaxTokenList WithoutEmptyText(this SyntaxTokenList tokens, SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.ValueText.IsNullOrWhiteSpace())
            {
                return tokens.Remove(token);
            }

            return tokens;
        }

        internal static SyntaxTokenList WithoutNewLine(this SyntaxTokenList tokens, SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
            {
                tokens = tokens.Remove(token);
            }

            return tokens;
        }

        internal static SyntaxToken WithText(this SyntaxToken token, string text) => SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), text, text, token.TrailingTrivia);

        internal static SyntaxToken WithTrailingEmptyLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingNewLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingXmlComment(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxNodeExtensions.XmlCommentStart);

        internal static SyntaxToken ToSyntaxToken(this string text, SyntaxKind kind = SyntaxKind.StringLiteralToken)
        {
            switch (kind)
            {
                case SyntaxKind.IdentifierToken:
                    return SyntaxFactory.Identifier(text);

                default:
                    return SyntaxFactory.Token(default, kind, text, text, default);
            }
        }
    }
}