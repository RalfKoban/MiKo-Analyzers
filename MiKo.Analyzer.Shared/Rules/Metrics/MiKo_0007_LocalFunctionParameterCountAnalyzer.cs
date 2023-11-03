using System.Linq;

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

        public MiKo_0007_LocalFunctionParameterCountAnalyzer() : base(Id, SyntaxKind.LocalFunctionStatement)
        {
        }

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LocalFunctionStatementSyntax localFunction)
            {
                var parameters = localFunction.ParameterList.Parameters;
                var parametersCount = parameters.Count;

                if (parametersCount > MaxParametersCount)
                {
                    var outParametersCount = parameters.Count(_ => _.Modifiers.Any(SyntaxKind.OutKeyword));
                    var count = parametersCount - outParametersCount;

                    if (count > MaxParametersCount)
                    {
                        var issue = Issue(localFunction.Identifier.GetLocation(), count, MaxParametersCount);

                        ReportDiagnostics(context, issue);
                    }
                }
            }
        }
    }
}