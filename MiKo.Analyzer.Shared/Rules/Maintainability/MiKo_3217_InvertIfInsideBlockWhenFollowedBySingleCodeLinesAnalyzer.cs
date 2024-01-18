using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3217_InvertIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3217";

        public MiKo_3217_InvertIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            // do not invert in case of an else block
            if (node.Else is null && node.Parent is BlockSyntax block)
            {
                var statements = block.Statements;

                var index = statements.IndexOf(node);

                if (index + 2 == statements.Count)
                {
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