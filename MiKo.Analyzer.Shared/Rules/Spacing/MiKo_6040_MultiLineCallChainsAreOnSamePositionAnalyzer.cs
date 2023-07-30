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

        public MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private static IReadOnlyCollection<SyntaxToken> CollectDots(InvocationExpressionSyntax invocation)
        {
            var dots = new Stack<SyntaxToken>();

            var expression = invocation.Expression;

            while (expression != null)
            {
                switch (expression)
                {
                    case MemberAccessExpressionSyntax s:
                        dots.Push(s.OperatorToken);
                        expression = s.Expression;

                        break;

                    case InvocationExpressionSyntax i:
                        expression = i.Expression;

                        break;

                    default:
                        expression = null;

                        break;
                }
            }

            return dots;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.Ancestors<InvocationExpressionSyntax>().Any())
            {
                // we are a nested invocation, hence we do not need to calculate and report again
                return;
            }

            var dots = CollectDots(invocation);

            var startPosition = dots.First().GetStartPosition();

            var startLine = startPosition.Line;
            var startCharacterPosition = startPosition.Character;

            foreach (var dot in dots)
            {
                var position = dot.GetStartPosition();

                if (position.Line != startLine && position.Character != startCharacterPosition)
                {
                    ReportDiagnostics(context, Issue(dot));
                }
            }
        }
    }
}