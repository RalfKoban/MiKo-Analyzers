using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0009_LoopsShallBeSmallAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0009";

        public MiKo_0009_LoopsShallBeSmallAnalyzer() : base(Id)
        {
        }

        public int MaxLinesOfCode { get; set; } = 10;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DoStatementSyntax node)
            {
                var lines = node.Statement.CountLines();

                if (lines > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(string.Empty, node.DoKeyword, lines, MaxLinesOfCode));
                }
            }
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is WhileStatementSyntax node)
            {
                var lines = node.Statement.CountLines();

                if (lines > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(string.Empty, node.WhileKeyword, lines, MaxLinesOfCode));
                }
            }
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ForStatementSyntax node)
            {
                var lines = node.Statement.CountLines();

                if (lines > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(string.Empty, node.ForKeyword, lines, MaxLinesOfCode));
                }
            }
        }

        private void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ForEachStatementSyntax node)
            {
                var lines = node.Statement.CountLines();

                if (lines > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(string.Empty, node.ForEachKeyword, lines, MaxLinesOfCode));
                }
            }
        }
    }
}