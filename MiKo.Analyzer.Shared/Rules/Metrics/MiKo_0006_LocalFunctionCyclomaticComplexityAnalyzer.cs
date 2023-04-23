using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0006";

        public MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzer() : base(Id, SyntaxKind.LocalFunctionStatement)
        {
        }

        public int MaxCyclomaticComplexity { get; set; } = 7;

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LocalFunctionStatementSyntax localFunction)
            {
                var cc = Counter.CountCyclomaticComplexity(localFunction.Body);

                if (cc > MaxCyclomaticComplexity)
                {
                    var identifier = localFunction.Identifier;

                    var issue = Issue(identifier.ValueText, identifier, cc, MaxCyclomaticComplexity);

                    ReportDiagnostics(context, issue);
                }
            }
        }
   }
}