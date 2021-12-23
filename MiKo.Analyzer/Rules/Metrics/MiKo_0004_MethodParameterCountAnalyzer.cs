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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            var parameterCount = symbol.Parameters.Length;
            if (parameterCount <= MaxParametersCount)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbol.IsExtern || symbol.IsInterfaceImplementation())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, parameterCount, MaxParametersCount) };
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}