using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.IsExtern is false)
            {
                var parameters = symbol.Parameters;
                var parametersCount = parameters.Length;

                if (parametersCount > MaxParametersCount && symbol.IsInterfaceImplementation() is false)
                {
                    var outParametersCount = parameters.Count(_ => _.RefKind == RefKind.Out);
                    var count = parametersCount - outParametersCount;

                    if (count > MaxParametersCount)
                    {
                        return new[] { Issue(symbol, count, MaxParametersCount) };
                    }
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}