using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3203_InvertNegativeIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer : InvertNegativeIfAnalyzer
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

        public MiKo_3203_InvertNegativeIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIfStatement(IfStatementSyntax node, SyntaxNodeAnalysisContext context)
        {
            // do not invert in case of an else block
            if (node.Else is null && node.Parent is BlockSyntax block)
            {
                var condition = node.Condition;
                var conditions = GetConditionParts(condition);

                if (condition.IsKind(SyntaxKind.LogicalAndExpression) && conditions.Count == 2 && conditions[0].IsKind(SyntaxKind.IsPatternExpression) && IsNegative(conditions[1]))
                {
                    // we do not want to report is-pattern checks for false because the inverted code looks much difficult to understand
                    return Enumerable.Empty<Diagnostic>();
                }

                if (conditions.Exists(IsNegative))
                {
                    var statements = block.Statements;

                    var index = statements.IndexOf(node);

                    if (statements.Count == index + 2)
                    {
                        var followUpStatement = statements[index + 1];

                        if (followUpStatement.IsAnyKind(ForbiddenFollowUps))
                        {
                            // we assume that the follow-up also contain code, so inverting would make the code less readable
                            return Enumerable.Empty<Diagnostic>();
                        }

                        // inspect only in case the if statement is followed by a single other statement
                        switch (node.Statement)
                        {
                            case ContinueStatementSyntax _:
                            case BlockSyntax b when b.Statements.FirstOrDefault() is ContinueStatementSyntax:
                                return new[] { Issue(node) };
                        }
                    }
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}