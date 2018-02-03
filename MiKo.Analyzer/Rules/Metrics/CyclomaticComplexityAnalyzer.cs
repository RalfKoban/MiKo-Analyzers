using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CyclomaticComplexityAnalyzer : MetricsAnalyzer
    {
        public CyclomaticComplexityAnalyzer() : base("MiKo_0002")
        {
        }

        public int MaxCyclomaticComplexity { get; set; } = 7;

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol)
        {
            var cc = CountCyclomaticComplexity(body);
            TryCreateDiagnostic(owningSymbol, cc, MaxCyclomaticComplexity, out var diagnostic);
            return diagnostic;
        }

        // if | while | for | foreach | case | continue | goto | && | || | catch | ternary operator ?: | ?? | ?.
        private static readonly SyntaxKind[] CCSyntaxKinds =
            {
                SyntaxKind.IfStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.ForStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.CaseSwitchLabel,
                SyntaxKind.CasePatternSwitchLabel,
                SyntaxKind.ContinueStatement,
                SyntaxKind.GotoStatement,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrExpression,
                SyntaxKind.CatchDeclaration,
                SyntaxKind.ConditionalExpression,
                SyntaxKind.CoalesceExpression,
                SyntaxKind.ConditionalAccessExpression,
            };

        private static int CountCyclomaticComplexity(BlockSyntax body)
        {
            var count = SyntaxNodeCollector<SyntaxNode>.Collect(body).Count(_ => CCSyntaxKinds.Any(_.IsKind));
            return 1 + count;
        }
    }
}