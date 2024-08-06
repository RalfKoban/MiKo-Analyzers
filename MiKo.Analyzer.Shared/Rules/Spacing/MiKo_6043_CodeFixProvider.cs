using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6043_CodeFixProvider)), Shared]
    public sealed class MiKo_6043_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6043";

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

        private static TypeArgumentListSyntax GetUpdatedSyntax(TypeArgumentListSyntax syntax)
        {
            if (syntax is null)
            {
                return null;
            }

            var arguments = syntax.Arguments;

            return syntax.WithoutTrivia()
                         .WithLessThanToken(syntax.LessThanToken.WithoutTrivia()) // remove the spaces or line breaks around the opening bracket
                         .WithArguments(SyntaxFactory.SeparatedList(arguments.Select(GetUpdatedSyntax), arguments.GetSeparators().Select(_ => _.WithoutTrivia().WithTrailingSpace()))) // fix separators
                         .WithGreaterThanToken(syntax.GreaterThanToken.WithoutTrivia()); // remove the spaces or line breaks around the closing bracket
        }

        private static InitializerExpressionSyntax GetUpdatedSyntax(InitializerExpressionSyntax syntax)
        {
            return syntax.WithoutTrivia()
                         .WithOpenBraceToken(syntax.OpenBraceToken.WithoutTrivia().WithLeadingSpace()) // remove the spaces or line breaks around the opening bracket
                         .WithCloseBraceToken(syntax.CloseBraceToken.WithoutTrivia().WithLeadingSpace()); // remove the spaces or line breaks around the closing bracket
        }

        private static ParameterListSyntax GetUpdatedSyntax(ParameterListSyntax syntax) => syntax.WithoutTrivia();

        private static ParameterSyntax GetUpdatedSyntax(ParameterSyntax syntax) => syntax.WithoutTrivia();

        private static TypeSyntax GetUpdatedSyntax(TypeSyntax syntax) => syntax.WithoutTrivia();

        private static SimpleNameSyntax GetUpdatedSyntax(SimpleNameSyntax syntax)
        {
            switch (syntax)
            {
                case IdentifierNameSyntax identifier: return GetUpdatedSyntax(identifier);
                case GenericNameSyntax generic: return GetUpdatedSyntax(generic);
                default:
                    return syntax?.WithoutTrivia();
            }
        }

        private static GenericNameSyntax GetUpdatedSyntax(GenericNameSyntax syntax) => syntax.WithIdentifier(syntax.Identifier)
                                                                                             .WithTypeArgumentList(GetUpdatedSyntax(syntax.TypeArgumentList));

        private static IdentifierNameSyntax GetUpdatedSyntax(IdentifierNameSyntax syntax) => syntax?.WithoutTrivia();

        private static SyntaxToken GetUpdatedSyntax(SyntaxToken token) => token.WithSurroundingSpace();

        private ParenthesizedLambdaExpressionSyntax GetUpdatedSyntax(ParenthesizedLambdaExpressionSyntax syntax) => syntax.WithParameterList(GetUpdatedSyntax(syntax.ParameterList))
                                                                                                                          .WithArrowToken(GetUpdatedSyntax(syntax.ArrowToken))
                                                                                                                          .WithExpressionBody(GetUpdatedSyntax(syntax.ExpressionBody))
                                                                                                                          .WithLeadingTriviaFrom(syntax);

        private SimpleLambdaExpressionSyntax GetUpdatedSyntax(SimpleLambdaExpressionSyntax syntax) => syntax.WithParameter(GetUpdatedSyntax(syntax.Parameter))
                                                                                                            .WithArrowToken(GetUpdatedSyntax(syntax.ArrowToken))
                                                                                                            .WithExpressionBody(GetUpdatedSyntax(syntax.ExpressionBody))
                                                                                                            .WithLeadingTriviaFrom(syntax);

        private ExpressionSyntax GetUpdatedSyntax(ExpressionSyntax syntax)
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

                case SimpleNameSyntax name:
                    return GetUpdatedSyntax(name);

                case ObjectCreationExpressionSyntax oces:
                    return oces.WithoutTrivia()
                               .WithNewKeyword(oces.NewKeyword.WithoutTrivia().WithTrailingSpace())
                               .WithType(GetUpdatedSyntax(oces.Type))
                               .WithInitializer(GetUpdatedSyntax(oces.Initializer));

                case AnonymousObjectCreationExpressionSyntax aoces:
                    return aoces.WithoutTrivia()
                                .WithNewKeyword(aoces.NewKeyword.WithoutTrivia().WithTrailingSpace())
                                .WithOpenBraceToken(aoces.OpenBraceToken.WithoutTrivia().WithLeadingSpace()) // remove the spaces or line breaks around the opening bracket
                                .WithCloseBraceToken(aoces.CloseBraceToken.WithoutTrivia().WithLeadingSpace()) // remove the spaces or line breaks around the closing bracket
                                .WithInitializers(GetUpdatedSyntax(aoces.Initializers, Constants.Indentation));

                case ParenthesizedLambdaExpressionSyntax p: return GetUpdatedSyntax(p);
                case SimpleLambdaExpressionSyntax s: return GetUpdatedSyntax(s);

                default:
                    return syntax?.WithoutTrivia();
            }
        }

        private ArgumentListSyntax GetUpdatedSyntax(ArgumentListSyntax syntax)
        {
            if (syntax is null)
            {
                return null;
            }

            var arguments = syntax.Arguments;

            return syntax.WithoutTrivia()
                         .WithOpenParenToken(syntax.OpenParenToken.WithoutTrivia()) // remove the spaces or line breaks around the opening parenthesis
                         .WithArguments(SyntaxFactory.SeparatedList(arguments.Select(GetUpdatedSyntax), arguments.GetSeparators().Select(_ => _.WithoutTrivia().WithTrailingSpace()))) // fix separators
                         .WithCloseParenToken(syntax.CloseParenToken.WithoutTrivia()); // remove the spaces or line breaks around the closing parenthesis
        }

        private ArgumentSyntax GetUpdatedSyntax(ArgumentSyntax syntax) => syntax.WithoutTrivia()
                                                                                .WithExpression(GetUpdatedSyntax(syntax.Expression));
    }
}