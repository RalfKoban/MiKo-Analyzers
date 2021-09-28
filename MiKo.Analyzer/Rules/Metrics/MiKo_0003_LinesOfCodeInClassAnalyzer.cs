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

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Struct:
                {
                    // ignore test classes
                    if (symbol.IsTestClass())
                    {
                        return Enumerable.Empty<Diagnostic>();
                    }

                    if (symbol.IsGenerated())
                    {
                        return Enumerable.Empty<Diagnostic>();
                    }

                    var loc = symbol.GetMembers().OfType<IMethodSymbol>()
                                    .Select(_ => _.GetSyntax())
                                    .SelectMany(SyntaxNodeCollector<BlockSyntax>.Collect)
                                    .Sum(Counter.CountLinesOfCode);

                    return TryCreateDiagnostic(symbol, loc, MaxLinesOfCode, out var diagnostic)
                               ? new[] { diagnostic }
                               : Enumerable.Empty<Diagnostic>();
                }

                // ignore interfaces
                case TypeKind.Interface:
                    return Enumerable.Empty<Diagnostic>();
                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}