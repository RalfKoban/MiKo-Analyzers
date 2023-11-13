using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3201";

        private const int MaximumAllowedFollowUpStatements = 3;

        private static readonly HashSet<SyntaxKind> ForbiddenFollowUps = new HashSet<SyntaxKind>
                                                                             {
                                                                                 SyntaxKind.DoStatement,
                                                                                 SyntaxKind.WhileStatement,
                                                                                 SyntaxKind.ForStatement,
                                                                                 SyntaxKind.ForEachStatement,
                                                                                 SyntaxKind.SwitchStatement,
                                                                                 SyntaxKind.IfStatement,
                                                                                 SyntaxKind.UsingStatement,
                                                                                 SyntaxKind.TryStatement,
                                                                                 SyntaxKind.LocalFunctionStatement,
                                                                             };

        public MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private static bool IsApplicable(IfStatementSyntax node) => node.Else is null // do not invert in case of an else block
                                                                 && node.ReturnsImmediately();

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            if (IsApplicable(node) is false)
            {
                return;
            }

            if (node.Parent is BlockSyntax block)
            {
                if (block.Parent is IfStatementSyntax)
                {
                    // do not invert nested ones
                    return;
                }

                var otherStatements = block.Statements.ToList();
                otherStatements.Remove(node); // get rid of the 'if' statement itself

                if (otherStatements.Count > 0 && otherStatements.Count <= MaximumAllowedFollowUpStatements)
                {
                    if (otherStatements.Any(_ => _.IsAnyKind(ForbiddenFollowUps)))
                    {
                        // we assume that those follow ups also contain code, so inverting would make the code less readable
                        return;
                    }

                    if (node.IsInsideLoop())
                    {
                        // inverting inside a loop would alter behavior
                        return;
                    }

                    if (node.FirstDescendant<ReturnStatementSyntax>().HasComment())
                    {
                        // the developer documented a reason, in that case we keep the if statement
                        return;
                    }

                    // report only in case we have something to invert
                    var method = node.GetEnclosingMethod(context.SemanticModel);

                    if (method != null && method.ReturnsVoid)
                    {
                        ReportDiagnostics(context, Issue(node));
                    }
                }
            }
        }
    }
}