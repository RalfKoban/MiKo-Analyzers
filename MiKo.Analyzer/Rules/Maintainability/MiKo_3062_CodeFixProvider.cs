﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3062_CodeFixProvider)), Shared]
    public sealed class MiKo_3062_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private const char FinalTextChar = ':';

        public override string FixableDiagnosticId => MiKo_3062_ExceptionLogMessageEndsWithColonAnalyzer.Id;

        protected override string Title => Resources.MiKo_3062_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            switch (syntax)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    return literal.WithToken(literal.Token.WithText($"\"{GetFixedText(literal.Token)}\""));

                case InterpolatedStringExpressionSyntax interpolated: // it's no text at the end, so add some
                    return interpolated.AddContents(SyntaxFactory.InterpolatedStringText(FinalTextChar.ToString().ToSyntaxToken(SyntaxKind.InterpolatedStringTextToken)));

                case InterpolatedStringTextSyntax text:
                    return text.WithTextToken(text.TextToken.WithText(GetFixedText(text.TextToken)));

                default:
                    return syntax;
            }
        }

        private static string GetFixedText(SyntaxToken token) => token.ValueText.TrimEnd() + FinalTextChar;
    }
}