using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0004_MethodParameterCountAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0004";

        private const int MaxParametersCount = 5;

        public MiKo_0004_MethodParameterCountAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsExtern) return Enumerable.Empty<Diagnostic>();

            var parameterCount = method.Parameters.Count();
            return parameterCount > MaxParametersCount
                       ? new[] { ReportIssue(method, parameterCount, MaxParametersCount) }
                       : Enumerable.Empty<Diagnostic>();
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}