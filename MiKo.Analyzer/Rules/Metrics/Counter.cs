﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    internal static class Counter
    {
        // if | do... while | while | for | foreach | case | switch expression arm | continue | goto | && | and | || | or | catch | catch when | ternary operator ?: | ?? | ??= | ?.
        private static readonly HashSet<int> CCSyntaxKinds = new HashSet<int>
            {
                (int)SyntaxKind.IfStatement,
                (int)SyntaxKind.DoStatement,
                (int)SyntaxKind.WhileStatement,
                (int)SyntaxKind.ForStatement,
                (int)SyntaxKind.ForEachStatement,
                (int)SyntaxKind.CaseSwitchLabel,
                (int)SyntaxKind.CasePatternSwitchLabel,
                (int)SyntaxKind.SwitchExpressionArm,
                (int)SyntaxKind.ContinueStatement,
                (int)SyntaxKind.GotoStatement,
                (int)SyntaxKind.LogicalAndExpression,
                (int)SyntaxKind.AndPattern,
                (int)SyntaxKind.LogicalOrExpression,
                (int)SyntaxKind.OrPattern,
                (int)SyntaxKind.CatchClause,
                (int)SyntaxKind.CatchFilterClause,
                (int)SyntaxKind.ConditionalExpression,
                (int)SyntaxKind.CoalesceExpression,
                (int)SyntaxKind.CoalesceAssignmentExpression,
                (int)SyntaxKind.ConditionalAccessExpression,
            };

        public static int CountCyclomaticComplexity(BlockSyntax body)
        {
            var count = SyntaxNodeCollector<SyntaxNode>.Collect(body).Count(_ => CCSyntaxKinds.Contains(_.RawKind));

            return 1 + count;
        }

        internal static int CountLinesOfCode(SyntaxNode body)
        {
            var nodes = SyntaxNodeCollector<StatementSyntax>.Collect(body);

            var lines = new HashSet<int>();
            CountLinesOfCode(nodes, lines);

            //// var all = string.Join(Environment.NewLine, lines.OrderBy(_ => _));

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
            while (true)
            {
                switch (node)
                {
                    case BlockSyntax _:
                        break;

                    case IfStatementSyntax s:
                        node = s.Condition;
                        continue;

                    case LocalDeclarationStatementSyntax s:
                        CountLinesOfCode(s.Declaration.GetLocation().GetLineSpan().StartLinePosition, lines);

                        // get normal initializers and object initializers
                        CountLinesOfCode(s.Declaration.Variables.Select(_ => _.Initializer?.Value).OfType<ObjectCreationExpressionSyntax>(), lines);
                        break;

                    case ObjectCreationExpressionSyntax s:

                        if (s.Initializer is null)
                        {
                            // it's a single line
                            CountLinesOfCode(s.GetLocation(), lines);
                        }
                        else
                        {
                            CountLinesOfCode(s.Initializer.Expressions, lines);
                        }

                        break;

                    case ReturnStatementSyntax s:
                        CountLinesOfCode(s.GetLocation().GetLineSpan().StartLinePosition, lines);
                        if (s.Expression != null)
                        {
                            node = s.Expression;
                            continue;
                        }

                        break;

                    case ForEachStatementSyntax s:
                        node = s.Expression;
                        continue;

                    case SwitchStatementSyntax s:
                        CountLinesOfCode(s.Expression, lines);
                        CountLinesOfCode(s.Sections.SelectMany(_ => _.Labels), lines);
                        break;

                    default:
                        CountLinesOfCode(node.GetLocation(), lines);
                        break;
                }

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