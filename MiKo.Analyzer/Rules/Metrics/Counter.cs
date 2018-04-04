using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    internal static class Counter
    {
        // if | do... while | while | for | foreach | case | continue | goto | && | || | catch | catch when | ternary operator ?: | ?? | ?.
        private static readonly SyntaxKind[] CCSyntaxKinds =
            {
                SyntaxKind.IfStatement,
                SyntaxKind.DoStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.ForStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.CaseSwitchLabel,
                SyntaxKind.CasePatternSwitchLabel,
                SyntaxKind.ContinueStatement,
                SyntaxKind.GotoStatement,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrExpression,
                SyntaxKind.CatchClause,
                SyntaxKind.CatchFilterClause,
                SyntaxKind.ConditionalExpression,
                SyntaxKind.CoalesceExpression,
                SyntaxKind.ConditionalAccessExpression,
            };

        public static int CountCyclomaticComplexity(BlockSyntax body)
        {
            var count = SyntaxNodeCollector<SyntaxNode>.Collect(body).Count(_ => CCSyntaxKinds.Any(_.IsKind));
            return 1 + count;
        }

        internal static int CountLinesOfCode(SyntaxNode body)
        {
            var nodes = SyntaxNodeCollector<StatementSyntax>.Collect(body);

            var lines = new HashSet<int>();
            CountLinesOfCode(nodes, lines);

            // var all = string.Join(Environment.NewLine, lines.OrderBy(_ => _));

            return lines.Count;
        }

        private static void CountLinesOfCode(IEnumerable<SyntaxNode> nodes, ISet<int> lines)
        {
            foreach (var node in nodes)
            {
                CountLinesOfCode(node, lines);
            }
        }

        private static void CountLinesOfCode(SyntaxNode node, ISet<int> lines)
        {
            switch (node)
            {
                case BlockSyntax _:
                    break;

                case ReturnStatementSyntax s:
                    CountLinesOfCode(s.GetLocation().GetLineSpan().StartLinePosition, lines);
                    if (s.Expression != null)
                        CountLinesOfCode(s.Expression, lines);
                    break;

                case IfStatementSyntax s:
                    CountLinesOfCode(s.Condition, lines);
                    break;

                case ForEachStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    break;

                case SwitchStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    CountLinesOfCode(s.Sections.SelectMany(_ => _.Labels), lines);
                    break;

                case LocalDeclarationStatementSyntax s:
                    CountLinesOfCode(s.Declaration.GetLocation().GetLineSpan().StartLinePosition, lines);

                    // get normal initializers and object initializers
                    CountLinesOfCode(s.Declaration.Variables.Select(_ => _.Initializer?.Value).OfType<ObjectCreationExpressionSyntax>(), lines);
                    break;

                case ObjectCreationExpressionSyntax oces:
                    if (oces.Initializer == null) // single line
                        CountLinesOfCode(oces.GetLocation(), lines);
                    else
                        CountLinesOfCode(oces.Initializer.Expressions, lines);
                    break;

                default:
                    CountLinesOfCode(node.GetLocation(), lines);
                    break;
            }
        }

        private static void CountLinesOfCode(LinePosition position, ISet<int> lines) => lines.Add(position.Line);

        private static void CountLinesOfCode(Location location, ISet<int> lines)
        {
            var lineSpan = location.GetLineSpan();
            CountLinesOfCode(lineSpan.StartLinePosition, lines);
            CountLinesOfCode(lineSpan.EndLinePosition, lines);
        }
    }
}