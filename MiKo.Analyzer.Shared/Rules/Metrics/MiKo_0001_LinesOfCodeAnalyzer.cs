using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0001_LinesOfCodeAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0001";

        public MiKo_0001_LinesOfCodeAnalyzer() : base(Id, DefaultSyntaxKinds)
        {
        }

        public int MaxLinesOfCode { get; set; } = 20;

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol containingSymbol)
        {
            var loc = Counter.CountLinesOfCode(body, SyntaxKind.LocalFunctionStatement);

            return loc > MaxLinesOfCode
                   ? Issue(containingSymbol, loc, MaxLinesOfCode)
                   : null;
        }
    }
}
