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

        private const string Spaces = "SPACES";
        private const string Difference = "DIFFERENCE";

        public MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer() : base(Id)
        {
        }

        internal static int GetSpaces(Diagnostic diagnostic) => int.Parse(diagnostic.Properties[Spaces]);

        internal static int GetDifference(Diagnostic diagnostic) => int.Parse(diagnostic.Properties[Difference]);

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);

        private static Stack<SyntaxToken> CollectDots(ExpressionSyntax startingExpression)
        {
            var dots = new Stack<SyntaxToken>();

            var expression = startingExpression;

            while (expression != null)
            {
                switch (expression)
                {
                    case MemberAccessExpressionSyntax s:
                    {
                        dots.Push(s.OperatorToken);

                        expression = s.Expression;

                        break;
                    }

                    case InvocationExpressionSyntax i:
                    {
                        expression = i.Expression;

                        break;
                    }

                    default:
                    {
                        expression = null;

                        break;
                    }
                }
            }

            return dots;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Ancestors<MemberAccessExpressionSyntax>().Any())
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
                        ReportDiagnostics(context, Issue(dot, new Dictionary<string, string>
                                                                  {
                                                                      { Spaces, startCharacterPosition.ToString("D") },
                                                                      { Difference, difference.ToString("D") },
                                                                  }));
                    }
                }
            }
        }
    }
}