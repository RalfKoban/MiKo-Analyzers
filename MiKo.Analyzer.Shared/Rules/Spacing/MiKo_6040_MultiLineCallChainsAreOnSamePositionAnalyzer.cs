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
            switch (expression)
            {
                case MemberAccessExpressionSyntax a:
                {
                    dots.Push(a.OperatorToken);

                    CollectDots(a.Expression, dots);

                    return;
                }

                case MemberBindingExpressionSyntax b:
                {
                    dots.Push(b.OperatorToken);

                    return;
                }

                case InvocationExpressionSyntax i:
                {
                    CollectDots(i.Expression, dots);

                    return;
                }

                case ConditionalAccessExpressionSyntax ca:
                {
                    CollectDots(ca.WhenNotNull, dots);
                    CollectDots(ca.Expression, dots);

                    return;
                }

                default:
                    return;
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (ExpressionSyntax)context.Node;

            if (node.Ancestors().Any(_ => _.IsAnyKind(Expressions)))
            {
                // we are a nested one, hence we do not need to calculate and report again
                return;
            }

            var dots = CollectDots(node);

            if (dots.None())
            {
                // no dots found for whatever reason, hence we do not need to report anything
                return;
            }

            var firstDot = dots.Pop();
            var startPosition = firstDot.GetStartPosition();
            var startLine = startPosition.Line;
            var startCharacterPosition = startPosition.Character;

            while (dots.Count > 0)
            {
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