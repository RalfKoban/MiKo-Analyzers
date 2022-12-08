using System.Collections.Generic;

using Microsoft.CodeAnalysis;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var localFunction in symbol.GetLocalFunctions())
            {
                var parameterCount = localFunction.ParameterList?.Parameters.Count;

                if (parameterCount > MaxParametersCount)
                {
                    yield return Issue(symbol, parameterCount, MaxParametersCount);
                }
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}