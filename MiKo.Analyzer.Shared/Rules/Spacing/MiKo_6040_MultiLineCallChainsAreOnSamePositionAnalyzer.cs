using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6040";

        private static readonly SyntaxKind[] Expressions = { SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ConditionalAccessExpression };

        public MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private static Stack<SyntaxToken> CollectDots(ExpressionSyntax startingExpression)
        {
            var dots = new Stack<SyntaxToken>();

            CollectDots(startingExpression, dots);

            return dots;
        }

        private static void CollectDots(ExpressionSyntax expression, Stack<SyntaxToken> dots)
        {
            while (true)
            {
                switch (expression)
                {
                    case MemberAccessExpressionSyntax a:
                    {
                        dots.Push(a.OperatorToken);

                        expression = a.Expression;

                        continue;
                    }

                    case InvocationExpressionSyntax i:
                    {
                        expression = i.Expression;

                        continue;
                    }

                    case ConditionalAccessExpressionSyntax ca:
                    {
                        CollectDots(ca.WhenNotNull, dots);

                        expression = ca.Expression;

                        continue;
                    }

                    case MemberBindingExpressionSyntax b:
                    {
                        dots.Push(b.OperatorToken);

                        return;
                    }

                    default:
                        return;
                }
            }
        }

        private static bool IsNested(ExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case StatementSyntax _:
                    case EqualsValueClauseSyntax _:
                    case InitializerExpressionSyntax _:
                    case MemberDeclarationSyntax _:
                    case ArrowExpressionClauseSyntax _:
                        return false;
                }

                if (ancestor.IsAnyKind(Expressions))
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (ExpressionSyntax)context.Node;

            if (IsNested(node))
            {
                // we are a nested one, hence we do not need to calculate and report again
                return;
            }

            var dots = CollectDots(node);

            if (dots.Count is 0)
            {
                // no dots found for whatever reason, hence we do not need to report anything
                return;
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var firstDot = dots.Pop();
            var startPosition = firstDot.GetStartPosition();
            var startLine = startPosition.Line;
            var startCharacterPosition = startPosition.Character;

            while (dots.Count > 0)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var dot = dots.Pop();

                if (dot.HasLeadingTrivia is false)
                {
                    // dot seems to have no leading spaces, most probably it is on the same line
                    continue;
                }

                var position = dot.GetStartPosition();

                if (position.Line != startLine)
                {
                    var difference = startCharacterPosition - position.Character;

                    if (difference != 0)
                    {
                        ReportDiagnostics(context, Issue(dot, CreateProposalForSpaces(startCharacterPosition, difference)));
                    }
                }
            }
        }
    }
}