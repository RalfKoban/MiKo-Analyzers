using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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

        private static ParenthesizedLambdaExpressionSyntax GetUpdatedSyntax(ParenthesizedLambdaExpressionSyntax syntax) => syntax.WithParameterList(GetUpdatedSyntax(syntax.ParameterList))
                                                                                                                                 .WithArrowToken(GetUpdatedSyntax(syntax.ArrowToken))
                                                                                                                                 .WithExpressionBody(GetUpdatedSyntax(syntax.ExpressionBody));

        private static SimpleLambdaExpressionSyntax GetUpdatedSyntax(SimpleLambdaExpressionSyntax syntax) => syntax.WithParameter(GetUpdatedSyntax(syntax.Parameter))
                                                                                                                   .WithArrowToken(GetUpdatedSyntax(syntax.ArrowToken))
                                                                                                                   .WithExpressionBody(GetUpdatedSyntax(syntax.ExpressionBody));

        private static ExpressionSyntax GetUpdatedSyntax(ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary:
                    return binary.WithoutTrivia()
                                 .WithLeft(GetUpdatedSyntax(binary.Left))
                                 .WithOperatorToken(GetUpdatedSyntax(binary.OperatorToken))
                                 .WithRight(GetUpdatedSyntax(binary.Right));

                case InvocationExpressionSyntax invocation:
                    return invocation.WithoutTrivia()
                                     .WithExpression(GetUpdatedSyntax(invocation.Expression))
                                     .WithArgumentList(GetUpdatedSyntax(invocation.ArgumentList));

                case MemberAccessExpressionSyntax maes:
                    return maes.WithoutTrivia()
                               .WithName(GetUpdatedSyntax(maes.Name))
                               .WithOperatorToken(maes.OperatorToken.WithoutTrivia()) // remove the spaces or line breaks around the dot
                               .WithExpression(GetUpdatedSyntax(maes.Expression));

                default:
                    return syntax.WithoutTrivia();
            }
        }

        private static ArgumentListSyntax GetUpdatedSyntax(ArgumentListSyntax syntax) => syntax?.WithoutTrivia()
                                                                                               .WithOpenParenToken(syntax.OpenParenToken.WithoutTrivia()) // remove the spaces or line breaks around the opening parenthesis
                                                                                               .WithArguments(SyntaxFactory.SeparatedList(syntax.Arguments.Select(GetUpdatedSyntax)))
                                                                                               .WithCloseParenToken(syntax.CloseParenToken.WithoutTrivia()); // remove the spaces or line breaks around the closing parenthesis

        private static ArgumentSyntax GetUpdatedSyntax(ArgumentSyntax syntax) => syntax.WithoutTrivia();

        private static ParameterListSyntax GetUpdatedSyntax(ParameterListSyntax syntax) => syntax.WithoutTrivia();

        private static ParameterSyntax GetUpdatedSyntax(ParameterSyntax syntax) => syntax.WithoutTrivia();

        private static T GetUpdatedSyntax<T>(T syntax) where T : NameSyntax => syntax?.WithoutTrivia();

        private static SyntaxToken GetUpdatedSyntax(SyntaxToken token) => token.WithSurroundingSpace();
    }
}