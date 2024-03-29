﻿using Microsoft.CodeAnalysis;
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

        private static readonly SyntaxKind[] Initializers = { SyntaxKind.ArrayInitializerExpression, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ObjectInitializerExpression };

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
                case ImplicitObjectCreationExpressionSyntax io: return io.ArgumentList.CloseParenToken.GetStartPosition();
                case AssignmentExpressionSyntax a: return GetAfterEndPosition(a.OperatorToken);

                // consider reduced array initializers
                case EqualsValueClauseSyntax e: return GetAfterEndPosition(e.EqualsToken);

                default:
                    return initializer.Parent.GetStartPosition();
            }

            LinePosition GetAfterEndPosition(SyntaxToken token)
            {
                var position = token.GetEndPosition();

                return new LinePosition(position.Line, position.Character + 1);
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InitializerExpressionSyntax initializer)
            {
                var openBraceToken = initializer.OpenBraceToken;

                var typePosition = GetStartPosition(initializer);
                var openBracePosition = openBraceToken.GetStartPosition();

                if (typePosition.Line != openBracePosition.Line && typePosition.Character != openBracePosition.Character)
                {
                    var issue = Issue(openBraceToken, CreateProposalForLinePosition(typePosition));

                    ReportDiagnostics(context, issue);
                }
            }
        }
    }
}