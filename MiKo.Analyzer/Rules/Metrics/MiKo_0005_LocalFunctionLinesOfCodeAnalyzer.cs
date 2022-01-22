using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var localFunction in symbol.GetSyntaxNodes().SelectMany(_ => _.DescendantNodes<LocalFunctionStatementSyntax>()))
            {
                var loc = Counter.CountLinesOfCode(localFunction.Body);

                if (loc > MaxLinesOfCode)
                {
                    yield return Issue(localFunction.GetName(), localFunction, loc, MaxLinesOfCode);
                }
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}
