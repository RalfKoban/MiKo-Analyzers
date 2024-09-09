using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    internal static class Counter
    {
        // if | do... while | while | for | foreach | case | switch expression arm | continue | goto | &= | & | && | and | |= | | | || | or | catch | catch when | ternary operator ?: | ?? | ??= | ?.
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
                                                                     (int)SyntaxKind.AndAssignmentExpression,
                                                                     (int)SyntaxKind.BitwiseAndExpression,
                                                                     (int)SyntaxKind.LogicalAndExpression,
                                                                     (int)SyntaxKind.AndPattern,
                                                                     (int)SyntaxKind.OrAssignmentExpression,
                                                                     (int)SyntaxKind.BitwiseOrExpression,
                                                                     (int)SyntaxKind.LogicalOrExpression,
                                                                     (int)SyntaxKind.OrPattern,
                                                                     (int)SyntaxKind.CatchClause,
                                                                     (int)SyntaxKind.CatchFilterClause,
                                                                     (int)SyntaxKind.ConditionalExpression,
                                                                     (int)SyntaxKind.CoalesceExpression,
                                                                     (int)SyntaxKind.CoalesceAssignmentExpression,
                                                                     (int)SyntaxKind.ConditionalAccessExpression,
                                                                 };

        public static int CountCyclomaticComplexity(BlockSyntax body, SyntaxKind syntaxKindToIgnore = SyntaxKind.None)
        {
            var count = SyntaxNodeCollector.Collect<SyntaxNode>(body, syntaxKindToIgnore).Count(_ => CCSyntaxKinds.Contains(_.RawKind));

            return 1 + count;
        }

        public static int CountCyclomaticComplexity(ArrowExpressionClauseSyntax body, SyntaxKind syntaxKindToIgnore = SyntaxKind.None)
        {
            var count = SyntaxNodeCollector.Collect<SyntaxNode>(body, syntaxKindToIgnore).Count(_ => CCSyntaxKinds.Contains(_.RawKind));

            return 1 + count;
        }

        internal static int CountLinesOfCode(SyntaxNode body, SyntaxKind syntaxKindToIgnore = SyntaxKind.None)
        {
            var nodes = SyntaxNodeCollector.Collect<StatementSyntax>(body, syntaxKindToIgnore);

            var lines = new HashSet<int>();

            CountLinesOfCode(nodes, lines);

            //// var all = string.Join(Environment.NewLine, lines.OrderBy(_ => _));

            return lines.Count;
        }

        private static void CountLinesOfCode(IReadOnlyList<SyntaxNode> nodes, ISet<int> lines)
        {
            var count = nodes.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    CountLinesOfCode(nodes[index], lines);
                }
            }
        }

        private static void CountLinesOfCode<T>(SeparatedSyntaxList<T> nodes, ISet<int> lines) where T : SyntaxNode
        {
            var count = nodes.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    CountLinesOfCode(nodes[index], lines);
                }
            }
        }

        private static void CountLinesOfCode<T>(SyntaxList<T> nodes, ISet<int> lines) where T : SyntaxNode
        {
            var count = nodes.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    CountLinesOfCode(nodes[index], lines);
                }
            }
        }

        private static void CountLinesOfCode(SyntaxNode node, ISet<int> lines)
        {
            while (true)
            {
                switch (node)
                {
                    case BlockSyntax _:
                    {
                        break;
                    }

                    case IfStatementSyntax s:
                    {
                        node = s.Condition;

                        continue;
                    }

                    case LocalDeclarationStatementSyntax s:
                    {
                        var declaration = s.Declaration;

                        CountLinesOfCode(declaration.GetStartingLine(), lines);

                        var variables = declaration.Variables;

                        var count = variables.Count;

                        if (count > 0)
                        {
                            // get normal initializers and object initializers
                            for (var index = 0; index < count; index++)
                            {
                                var variable = variables[index];

                                if (variable.Initializer?.Value is ObjectCreationExpressionSyntax syntax)
                                {
                                    CountLinesOfCode(syntax, lines);
                                }
                            }
                        }

                        break;
                    }

                    case ObjectCreationExpressionSyntax s:
                    {
                        var initializer = s.Initializer;

                        if (initializer is null)
                        {
                            // it's a single line
                            CountLinesOfCode(s.GetLocation(), lines);
                        }
                        else
                        {
                            CountLinesOfCode(initializer.Expressions, lines);
                        }

                        break;
                    }

                    case ReturnStatementSyntax s:
                    {
                        CountLinesOfCode(s.GetStartingLine(), lines);

                        var expression = s.Expression;

                        if (expression is null)
                        {
                            break;
                        }

                        node = expression;

                        continue;
                    }

                    case ForEachStatementSyntax s:
                    {
                        node = s.Expression;

                        continue;
                    }

                    case SwitchStatementSyntax s:
                    {
                        CountLinesOfCode(s.Expression, lines);

                        var sections = s.Sections;
                        var count = sections.Count;

                        if (count > 0)
                        {
                            for (var index = 0; index < count; index++)
                            {
                                CountLinesOfCode(sections[index].Labels, lines);
                            }
                        }

                        break;
                    }

                    default:
                    {
                        CountLinesOfCode(node.GetLocation(), lines);

                        break;
                    }
                }

                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountLinesOfCode(LinePosition position, ISet<int> lines) => CountLinesOfCode(position.Line, lines);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountLinesOfCode(Location location, ISet<int> lines)
        {
            var lineSpan = location.GetLineSpan();

            CountLinesOfCode(lineSpan.StartLinePosition, lines);
            CountLinesOfCode(lineSpan.EndLinePosition, lines);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountLinesOfCode(int line, ISet<int> lines) => lines.Add(line);
    }
}