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

        private static GenericNameSyntax GetUpdatedSyntax(GenericNameSyntax syntax) => syntax.WithIdentifier(syntax.Identifier)
                                                                                             .WithTypeArgumentList(GetUpdatedSyntax(syntax.TypeArgumentList));

        private static IdentifierNameSyntax GetUpdatedSyntax(IdentifierNameSyntax syntax) => syntax?.WithoutTrivia();

        private static InitializerExpressionSyntax GetUpdatedSyntax(InitializerExpressionSyntax syntax)
        {
            return syntax.WithoutTrivia()
                         .WithOpenBraceToken(syntax.OpenBraceToken.WithoutTrivia().WithLeadingSpace()) // remove the spaces or line breaks around the opening bracket
                         .WithCloseBraceToken(syntax.CloseBraceToken.WithoutTrivia().WithLeadingSpace()); // remove the spaces or line breaks around the closing bracket
        }

        private static ObjectCreationExpressionSyntax GetUpdatedSyntax(ObjectCreationExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                       .WithNewKeyword(syntax.NewKeyword.WithoutTrivia().WithTrailingSpace())
                                                                                                                       .WithType(GetUpdatedSyntax(syntax.Type))
                                                                                                                       .WithInitializer(GetUpdatedSyntax(syntax.Initializer));

        private static ParameterSyntax GetUpdatedSyntax(ParameterSyntax syntax) => syntax.WithoutTrivia();

        private static ParameterListSyntax GetUpdatedSyntax(ParameterListSyntax syntax) => syntax.WithoutTrivia();

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

        private static TypeSyntax GetUpdatedSyntax(TypeSyntax syntax) => syntax.WithoutTrivia();

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

        private static SyntaxToken GetUpdatedSyntax(in SyntaxToken token) => token.WithSurroundingSpace();

        private static AnonymousObjectCreationExpressionSyntax GetUpdatedSyntax(AnonymousObjectCreationExpressionSyntax syntax)
        {
            return syntax.WithoutTrivia()
                         .WithNewKeyword(syntax.NewKeyword.WithoutTrivia().WithTrailingSpace())
                         .WithOpenBraceToken(syntax.OpenBraceToken.WithoutTrivia().WithLeadingSpace()) // remove the spaces or line breaks around the opening bracket
                         .WithCloseBraceToken(syntax.CloseBraceToken.WithoutTrivia().WithLeadingSpace()) // remove the spaces or line breaks around the closing bracket
                         .WithInitializers(GetUpdatedSyntax(syntax.Initializers, syntax.OpenBraceToken, Constants.Indentation));
        }

        private static ArgumentSyntax GetUpdatedSyntax(ArgumentSyntax syntax) => syntax.WithoutTrivia()
                                                                                       .WithExpression(GetUpdatedSyntax(syntax.Expression));

        private static ArgumentListSyntax GetUpdatedSyntax(ArgumentListSyntax syntax)
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

        private static BinaryExpressionSyntax GetUpdatedSyntax(BinaryExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                       .WithLeft(GetUpdatedSyntax(syntax.Left))
                                                                                                       .WithOperatorToken(GetUpdatedSyntax(syntax.OperatorToken))
                                                                                                       .WithRight(GetUpdatedSyntax(syntax.Right));

        private static ConditionalAccessExpressionSyntax GetUpdatedSyntax(ConditionalAccessExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                             .WithOperatorToken(syntax.OperatorToken.WithoutTrivia())
                                                                                                                             .WithWhenNotNull(GetUpdatedSyntax(syntax.WhenNotNull))
                                                                                                                             .WithExpression(GetUpdatedSyntax(syntax.Expression));

        private static ExpressionSyntax GetUpdatedSyntax(ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary: return GetUpdatedSyntax(binary);
                case InvocationExpressionSyntax invocation: return GetUpdatedSyntax(invocation);
                case MemberAccessExpressionSyntax maes: return GetUpdatedSyntax(maes);
                case SimpleNameSyntax name: return GetUpdatedSyntax(name);
                case ObjectCreationExpressionSyntax oces: return GetUpdatedSyntax(oces);
                case AnonymousObjectCreationExpressionSyntax aoces: return GetUpdatedSyntax(aoces);
                case ConditionalAccessExpressionSyntax conditional: return GetUpdatedSyntax(conditional);
                case ParenthesizedLambdaExpressionSyntax p: return GetUpdatedSyntax(p);
                case SimpleLambdaExpressionSyntax s: return GetUpdatedSyntax(s);

                default:
                    return syntax?.WithoutTrivia();
            }
        }

        private static InvocationExpressionSyntax GetUpdatedSyntax(InvocationExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                               .WithExpression(GetUpdatedSyntax(syntax.Expression))
                                                                                                               .WithArgumentList(GetUpdatedSyntax(syntax.ArgumentList));

        private static MemberAccessExpressionSyntax GetUpdatedSyntax(MemberAccessExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                   .WithName(GetUpdatedSyntax(syntax.Name))
                                                                                                                   .WithOperatorToken(syntax.OperatorToken.WithoutTrivia()) // remove the spaces or line breaks around the dot
                                                                                                                   .WithExpression(GetUpdatedSyntax(syntax.Expression));

        private static ParenthesizedLambdaExpressionSyntax GetUpdatedSyntax(ParenthesizedLambdaExpressionSyntax syntax) => syntax.WithParameterList(GetUpdatedSyntax(syntax.ParameterList))
                                                                                                                                 .WithArrowToken(GetUpdatedSyntax(syntax.ArrowToken))
                                                                                                                                 .WithExpressionBody(GetUpdatedSyntax(syntax.ExpressionBody))
                                                                                                                                 .WithLeadingTriviaFrom(syntax);

        private static SimpleLambdaExpressionSyntax GetUpdatedSyntax(SimpleLambdaExpressionSyntax syntax) => syntax.WithParameter(GetUpdatedSyntax(syntax.Parameter))
                                                                                                                   .WithArrowToken(GetUpdatedSyntax(syntax.ArrowToken))
                                                                                                                   .WithExpressionBody(GetUpdatedSyntax(syntax.ExpressionBody))
                                                                                                                   .WithLeadingTriviaFrom(syntax);
    }
}