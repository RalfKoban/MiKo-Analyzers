using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6073_LinqQueriesAreVerticallyAlignedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6073";

        public MiKo_6073_LinqQueriesAreVerticallyAlignedAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQueryExpression, SyntaxKind.QueryExpression);

        private static IEnumerable<SyntaxNode> GetProblematicNodes(QueryBodySyntax body, LinePosition startPosition)
        {
            var currentBody = body;

            while (true)
            {
                foreach (var clause in currentBody.Clauses)
                {
                    var clausePosition = clause.GetStartPosition();

                    if (NotVerticallyAligned(startPosition, clausePosition))
                    {
                        yield return clause;
                    }
                }

                var selectPart = currentBody.SelectOrGroup;
                var selectPartPosition = selectPart.GetStartPosition();

                if (NotVerticallyAligned(startPosition, selectPartPosition))
                {
                    yield return selectPart;
                }

                var continuationPart = currentBody.Continuation;

                if (continuationPart is null)
                {
                    // nothing more to report
                    yield break;
                }

                currentBody = continuationPart.Body;

                var continuationPartPosition = continuationPart.GetStartPosition();

                if (selectPartPosition.Line != continuationPartPosition.Line && NotVerticallyAligned(startPosition, continuationPartPosition))
                {
                    yield return continuationPart;
                }
            }
        }

        private void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is QueryExpressionSyntax node)
            {
                var startPosition = node.FromClause.GetStartPosition();
                var problematicNodes = GetProblematicNodes(node.Body, startPosition);

                foreach (var problematicNode in problematicNodes)
                {
                    ReportDiagnostics(context, Issue(problematicNode, CreateProposalForSpaces(startPosition.Character)));
                }
            }
        }
    }
}