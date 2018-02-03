using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LinesOfCodeAnalyzer : MetricsAnalyzer
    {
        public LinesOfCodeAnalyzer() : base("MiKo_Metric_0001")
        {
        }

        public int MaxLinesOfCode { get; set; } = 20;

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol)
        {
            var loc = CountLinesOfCode(body);
            TryCreateDiagnostic(owningSymbol, loc, MaxLinesOfCode, out var diagnostic);
            return diagnostic;
        }

        private static int CountLinesOfCode(SyntaxNode body)
        {
            var lines = new HashSet<int>();

            foreach (var node in SyntaxNodeCollector<StatementSyntax>.Collect(body))
            {
                CountLinesOfCode(node, lines);
            }

            return lines.Count;
        }

        private static void CountLinesOfCode(SyntaxNode node, ISet<int> lines)
        {
            while (node != null)
            {
                switch (node)
                {
                    case BlockSyntax _:
                        return;

                    case ForEachStatementSyntax s:
                        node = s.Expression;
                        continue;

                    case SwitchStatementSyntax s:
                        node = s.Expression;
                        continue;

                    default:
                        var lineSpan = node.GetLocation().GetLineSpan();
                        lines.Add(lineSpan.StartLinePosition.Line);
                        lines.Add(lineSpan.EndLinePosition.Line);
                        return;
                }
            }
        }
    }
}
