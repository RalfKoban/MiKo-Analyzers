using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6054_CodeFixProvider)), Shared]
    public sealed class MiKo_6054_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6054";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case ParenthesizedLambdaExpressionSyntax _:
                    case SimpleLambdaExpressionSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case ParenthesizedLambdaExpressionSyntax p: return GetUpdatedSyntax(p);
                case SimpleLambdaExpressionSyntax s: return GetUpdatedSyntax(s);

                default:
                    // we cannot fix it
                    return syntax;
            }
        }

        private static ParenthesizedLambdaExpressionSyntax GetUpdatedSyntax(ParenthesizedLambdaExpressionSyntax syntax)
        {
            var updatedSyntax = syntax.WithParameterList(syntax.ParameterList.WithoutTrailingTrivia())
                                      .WithArrowToken(syntax.ArrowToken.WithoutTrivia().WithLeadingSpace())
                                      .WithExpressionBody(syntax.ExpressionBody?.WithoutLeadingTrivia().WithLeadingSpace());

            return UpdateExpressionBody(syntax, updatedSyntax);
        }

        private static SimpleLambdaExpressionSyntax GetUpdatedSyntax(SimpleLambdaExpressionSyntax syntax)
        {
            var updatedSyntax = syntax.WithParameter(syntax.Parameter.WithoutTrailingTrivia())
                                      .WithArrowToken(syntax.ArrowToken.WithoutTrivia().WithLeadingSpace())
                                      .WithExpressionBody(syntax.ExpressionBody?.WithoutLeadingTrivia().WithLeadingSpace());

            return UpdateExpressionBody(syntax, updatedSyntax);
        }

        private static T UpdateExpressionBody<T>(T syntax, T updatedSyntax) where T : AnonymousFunctionExpressionSyntax
        {
            var expressionBody = syntax.ExpressionBody;

            if (expressionBody is null)
            {
                return updatedSyntax;
            }

            var initialSpaces = syntax.GetPositionWithinStartLine();
            var additionalSpaces = updatedSyntax.ExpressionBody.GetPositionWithinStartLine();
            var spaces = initialSpaces + additionalSpaces;

            var updatedExpression = GetSyntaxWithLeadingSpaces(expressionBody, spaces);

            return (T)updatedSyntax.WithExpressionBody(updatedExpression);
        }
    }
}