using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6030";

        private static readonly SyntaxKind[] Initializers = { SyntaxKind.ArrayInitializerExpression, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ObjectInitializerExpression, SyntaxKind.AnonymousObjectCreationExpression };

        public MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Initializers);

        private static LinePosition GetStartPosition(InitializerExpressionSyntax initializer)
        {
            switch (initializer.Parent)
            {
                case ArrayCreationExpressionSyntax a: return a.Type.GetStartPosition();
                case ObjectCreationExpressionSyntax o: return o.Type.GetStartPosition();
                case ImplicitArrayCreationExpressionSyntax ia: return ia.CloseBracketToken.GetStartPosition();
                case ImplicitObjectCreationExpressionSyntax io:
                {
                    var arguments = io.ArgumentList;

                    return arguments.Arguments.Count > 0
                           ? arguments.OpenParenToken.GetEndPosition()
                           : arguments.CloseParenToken.GetStartPosition();
                }

                case AssignmentExpressionSyntax a: return a.OperatorToken.GetPositionAfterEnd();

                // consider reduced array initializers
                case EqualsValueClauseSyntax e: return e.EqualsToken.GetPositionAfterEnd();

                default:
                    return initializer.Parent.GetStartPosition();
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeNode(context.Node);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeNode(SyntaxNode node)
        {
            switch (node)
            {
                case InitializerExpressionSyntax initializer:
                {
                    var openBraceToken = initializer.OpenBraceToken;

                    var typePosition = GetStartPosition(initializer);

                    return NotVerticallyAligned(openBraceToken, typePosition)
                           ? Issue(openBraceToken, CreateProposalForSpaces(typePosition.Character))
                           : null;
                }

                case AnonymousObjectCreationExpressionSyntax anonymous:
                {
                    var openBraceToken = anonymous.OpenBraceToken;

                    var keywordPosition = anonymous.NewKeyword.GetPositionAfterEnd();

                    return NotVerticallyAligned(openBraceToken, keywordPosition)
                           ? Issue(openBraceToken, CreateProposalForSpaces(keywordPosition.Character))
                           : null;
                }

                default:
                    return null;
            }
        }
    }
}