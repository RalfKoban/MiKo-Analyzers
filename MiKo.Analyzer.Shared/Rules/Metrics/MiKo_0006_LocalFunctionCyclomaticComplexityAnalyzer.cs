using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0006";

        public MiKo_0006_LocalFunctionCyclomaticComplexityAnalyzer() : base(Id)
        {
        }

        public int MaxCyclomaticComplexity { get; set; } = 7;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var localFunction in symbol.GetLocalFunctions())
            {
                var cc = Counter.CountCyclomaticComplexity(localFunction.Body);

                if (cc > MaxCyclomaticComplexity)
                {
                    var identifier = localFunction.Identifier;

                    yield return Issue(identifier.ValueText, identifier, cc, MaxCyclomaticComplexity);
                }
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}