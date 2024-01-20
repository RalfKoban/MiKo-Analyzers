using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3203_InvertNegativeIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3203";

        private static readonly ISet<SyntaxKind> ForbiddenFollowUps = new HashSet<SyntaxKind>
                                                                          {
                                                                              SyntaxKind.DoStatement,
                                                                              SyntaxKind.WhileStatement,
                                                                              SyntaxKind.ForStatement,
                                                                              SyntaxKind.ForEachStatement,
                                                                              SyntaxKind.TryStatement,
                                                                              SyntaxKind.UsingStatement,
                                                                              SyntaxKind.IfStatement,
                                                                              SyntaxKind.LocalFunctionStatement,
                                                                          };

        public MiKo_3203_InvertNegativeIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private static bool IsNegative(SyntaxNode condition)
        {
            foreach (var node in condition.DescendantNodesAndSelf())
            {
                if (node.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    return true;
                }

                if (node is IsPatternExpressionSyntax pattern && pattern.Pattern is ConstantPatternSyntax c && c.Expression.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            // do not invert in case of an else block
            if (node.Else is null && node.Parent is BlockSyntax block && IsNegative(node.Condition))
            {
                var statements = block.Statements;

                var index = statements.IndexOf(node);

                if (index + 2 == statements.Count)
                {
                    if (statements[index + 1].IsAnyKind(ForbiddenFollowUps))
                    {
                        // we assume that the follow-up also contain code, so inverting would make the code less readable
                        return;
                    }

                    // inspect only in case the if statement is followed by a single other statement
                    switch (node.Statement)
                    {
                        case ContinueStatementSyntax _:
                        case BlockSyntax b when b.Statements.FirstOrDefault() is ContinueStatementSyntax:
                            ReportDiagnostics(context, Issue(node));

                            break;
                    }
                }
            }
        }
    }
}