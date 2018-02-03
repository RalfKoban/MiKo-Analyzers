using System.Collections.Generic;
using System.Linq;

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

            return loc > MaxLinesOfCode
                    ? Diagnostic.Create(Rule, owningSymbol.Locations.First(), owningSymbol.Name, loc, MaxLinesOfCode)
                    : null;
        }

        private static int CountLinesOfCode(SyntaxNode body)
        {
            var collector = new SyntaxNodeCollector<StatementSyntax>();
            collector.Visit(body);

            var lines = new HashSet<int>();

            foreach (var node in collector.Nodes)
            {
                CountLinesOfCode(node, lines);
            }

            return lines.Count;
        }

        private static void CountLinesOfCode(SyntaxNode node, ISet<int> lines)
        {
            switch (node)
            {
                case BlockSyntax _: return;

                case ForEachStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    return;

                case SwitchStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    return;

                default:
                    var lineSpan = node.GetLocation().GetLineSpan();
                    lines.Add(lineSpan.StartLinePosition.Line);
                    lines.Add(lineSpan.EndLinePosition.Line);
                    return;
            }

        }
    }
}
