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

        public MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private static bool ReturnsImmediately(IfStatementSyntax node)
        {
            switch (node.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsApplicable(IfStatementSyntax node) => node.Else == null // do not invert in case of an else block
                                                                 && ReturnsImmediately(node);

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

                var statements = block.Statements;
                var otherStatementsCount = statements.Count - 1; // subtract 1 for the 'if' statement itself

                if (otherStatementsCount > 0 && otherStatementsCount <= MaximumAllowedFollowUpStatements)
                {
                    // report only in case we have something to invert
                    if (node.IsInsideLoop() is false)
                    {
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
}