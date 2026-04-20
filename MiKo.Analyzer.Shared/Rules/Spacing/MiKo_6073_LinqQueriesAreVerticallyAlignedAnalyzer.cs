using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

        private void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is QueryExpressionSyntax node)
            {
                var issues = AnalyzeQuery(node);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeQuery(QueryExpressionSyntax node)
        {
            var startPosition = node.FromClause.GetStartPosition();
            var startPositionCharacter = startPosition.Character;

            foreach (var clause in node.Body.Clauses)
            {
                var clausePosition = clause.GetStartPosition();

                if (NotVerticallyAligned(startPosition, clausePosition))
                {
                    yield return Issue(clause, CreateProposalForSpaces(startPositionCharacter));
                }
            }

            var selectPart = node.Body.SelectOrGroup;
            var selectPartPosition = selectPart.GetStartPosition();

            if (NotVerticallyAligned(startPosition, selectPartPosition))
            {
                yield return Issue(selectPart, CreateProposalForSpaces(startPositionCharacter));
            }

            var continuationPart = node.Body.Continuation;

            if (continuationPart != null)
            {
                var continuationPartPosition = continuationPart.GetStartPosition();

                if (selectPartPosition.Line != continuationPartPosition.Line && NotVerticallyAligned(startPosition, continuationPartPosition))
                {
                    yield return Issue(continuationPart, CreateProposalForSpaces(startPositionCharacter));
                }
            }
        }
    }
}