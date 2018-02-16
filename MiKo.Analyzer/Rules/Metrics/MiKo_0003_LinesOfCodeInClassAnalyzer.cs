using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0003_LinesOfCodeInClassAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0003";

        public MiKo_0003_LinesOfCodeInClassAnalyzer() : base(Id)
        {
        }

        public int MaxLinesOfCode { get; set; } = 220;

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            var loc = symbol
                      .GetMembers().OfType<IMethodSymbol>()
                      .SelectMany(_ => _.DeclaringSyntaxReferences.Select(__ => __.GetSyntaxAsync().Result).SelectMany(SyntaxNodeCollector<BlockSyntax>.Collect))
                      .Sum(Counter.CountLinesOfCode);

            TryCreateDiagnostic(symbol, loc, MaxLinesOfCode, out var diagnostic);
            return diagnostic != null ? new [] { diagnostic } : Enumerable.Empty<Diagnostic>();
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}