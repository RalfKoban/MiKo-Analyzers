using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0005_LocalFunctionLinesOfCodeAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0005";

        public MiKo_0005_LocalFunctionLinesOfCodeAnalyzer() : base(Id)
        {
        }

        public int MaxLinesOfCode { get; set; } = 20;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.LocalFunctionStatement);

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LocalFunctionStatementSyntax localFunction)
            {
                var loc = Counter.CountLinesOfCode(localFunction.Body);

                if (loc > MaxLinesOfCode)
                {
                    var issue = Issue(localFunction.GetName(), localFunction.Identifier, loc, MaxLinesOfCode);

                    ReportDiagnostics(context, issue);
                }
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}
