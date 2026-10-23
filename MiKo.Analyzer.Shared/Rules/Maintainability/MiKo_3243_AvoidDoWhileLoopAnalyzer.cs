using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3243_AvoidDoWhileLoopAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3243";

        public MiKo_3243_AvoidDoWhileLoopAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DoStatementSyntax node)
            {
                ReportDiagnostics(context, Issue(node.DoKeyword));
            }
        }
    }
}