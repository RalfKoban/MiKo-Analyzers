using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3303_CodeFixProvider)), Shared]
    public sealed class MiKo_3303_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3303_LambdaExpressionBodiesAreOnSameLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_3303_CodeFixTitle;

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

        private static ParenthesizedLambdaExpressionSyntax GetUpdatedSyntax(ParenthesizedLambdaExpressionSyntax lambda) => lambda.WithParameterList(lambda.ParameterList.WithoutTrivia())
                                                                                                                          .WithArrowToken(lambda.ArrowToken.WithSurroundingSpace())
                                                                                                                          .WithExpressionBody(GetUpdatedSyntax(lambda.ExpressionBody));

        private static SimpleLambdaExpressionSyntax GetUpdatedSyntax(SimpleLambdaExpressionSyntax lambda) => lambda.WithParameter(lambda.Parameter.WithoutTrivia())
                                                                                                                   .WithArrowToken(lambda.ArrowToken.WithSurroundingSpace())
                                                                                                                   .WithExpressionBody(GetUpdatedSyntax(lambda.ExpressionBody));

        private static ExpressionSyntax GetUpdatedSyntax(ExpressionSyntax expression)
        {
            if (expression is BinaryExpressionSyntax binary)
            {
                return binary.WithoutTrivia()
                             .WithLeft(GetUpdatedSyntax(binary.Left))
                             .WithOperatorToken(binary.OperatorToken.WithSurroundingSpace())
                             .WithRight(GetUpdatedSyntax(binary.Right));
            }

            return expression.WithoutTrivia();
        }
    }
}