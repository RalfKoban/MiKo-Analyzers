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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Struct:
                    {
                        if (symbol.IsGenerated())
                        {
                            yield break;
                        }

                        // ignore test classes
                        if (symbol.IsTestClass())
                        {
                            yield break;
                        }

                        var methods = symbol.GetMethods()
                                            .Where(_ => _.CanBeReferencedByName || _.IsConstructor())
                                            .Where(_ => _.Locations.Any(__ => __.IsInSource))
                                            .Select(_ => _.GetSyntax());

                        var properties = symbol.GetProperties()
                                               .Where(_ => _.Locations.Any(__ => __.IsInSource))
                                               .Select(_ => _.GetSyntax());

                        var loc = methods.Concat(properties)
                                  .SelectMany(_ => SyntaxNodeCollector.Collect<BlockSyntax>(_))
                                  .Sum(_ => Counter.CountLinesOfCode(_));

                        if (loc > MaxLinesOfCode)
                        {
                            yield return Issue(symbol, loc, MaxLinesOfCode);
                        }

                        yield break;
                    }

                // ignore interfaces
                case TypeKind.Interface:
                    yield break;

                default:
                    yield break;
            }
        }

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol) => null;
    }
}