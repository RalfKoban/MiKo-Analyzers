using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0007_LocalFunctionParameterCountAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0007";

        private const int MaxParametersCount = 3;

        public MiKo_0007_LocalFunctionParameterCountAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.LocalFunctionStatement);

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LocalFunctionStatementSyntax localFunction)
            {
                var parameterCount = localFunction.ParameterList?.Parameters.Count;

                if (parameterCount > MaxParametersCount)
                {
                    var issue = Issue(localFunction.Identifier.GetLocation(), parameterCount, MaxParametersCount);

                    ReportDiagnostics(context, issue);
                }
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}