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

        private const int MaximumAllowedFollowUpStatements = 3 + 1; // incl. 1 for the 'if' statement itself

        public MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private static bool IfStatementContains<T>(IfStatementSyntax node) where T : StatementSyntax
        {
            switch (node.Statement)
            {
                case T _:
                case BlockSyntax block when block.Statements.Any(_ => _ is T):
                    return true;

                default:
                    return false;
            }
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            if (node.Else != null)
            {
                // do not invert in case of an else block
                return;
            }

            if (IfStatementContains<ThrowStatementSyntax>(node))
            {
                // do not invert if method throws
                return;
            }

            if (IfStatementContains<IfStatementSyntax>(node))
            {
                // do not invert if condition contains another if
                return;
            }

            if (node.Parent is BlockSyntax block)
            {
                if (block.Parent is IfStatementSyntax)
                {
                    // do not invert nested ones
                    return;
                }

                if (block.Statements.Count <= MaximumAllowedFollowUpStatements)
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