using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class LinesOfCodeAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0001";

        public LinesOfCodeAnalyzer() : base(Id)
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
            var nodes = SyntaxNodeCollector<StatementSyntax>.Collect(body);

            var lines = new HashSet<int>();
            CountLinesOfCode(nodes, lines);
            return lines.Count;
        }

        private static void CountLinesOfCode(IEnumerable<SyntaxNode> nodes, ISet<int> lines)
        {
            foreach (var node in nodes)
            {
                CountLinesOfCode(node, lines);
            }
        }

        private static void CountLinesOfCode(SyntaxNode node, ISet<int> lines)
        {
            switch (node)
            {
                case BlockSyntax _:
                    break;

                case IfStatementSyntax s:
                    CountLinesOfCode(s.Condition, lines);
                    break;

                case ForEachStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    break;

                case SwitchStatementSyntax s:
                    CountLinesOfCode(s.Expression, lines);
                    CountLinesOfCode(s.Sections.SelectMany(_ => _.Labels), lines);
                    break;

                default:
                    CountLinesOfCode(node.GetLocation(), lines);
                    break;
            }
        }

        private static void CountLinesOfCode(Location location, ISet<int> lines)
        {
            var lineSpan = location.GetLineSpan();
            lines.Add(lineSpan.StartLinePosition.Line);
            lines.Add(lineSpan.EndLinePosition.Line);
        }
    }
}
