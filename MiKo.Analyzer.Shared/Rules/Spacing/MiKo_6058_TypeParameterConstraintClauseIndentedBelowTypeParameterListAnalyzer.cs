using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6058_TypeParameterConstraintClauseIndentedBelowTypeParameterListAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6058";

        private const int Offset = Constants.Indentation / 2;

        public MiKo_6058_TypeParameterConstraintClauseIndentedBelowTypeParameterListAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeTypeParameterConstraintClause, SyntaxKind.TypeParameterConstraintClause);

        private void AnalyzeTypeParameterConstraintClause(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is TypeParameterConstraintClauseSyntax node)
            {
                var issues = AnalyzeTypeParameterConstraintClause(node);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            var clauses = node.GetConstraintClauses();

            if (clauses.IndexOf(node) > 0)
            {
                // only report for the first one
                return Enumerable.Empty<Diagnostic>();
            }

            var typeParameters = node.GetTypeParameterList();
            var referencePosition = typeParameters.LessThanToken.GetStartPosition();

            var whereKeyword = node.WhereKeyword;
            var position = whereKeyword.GetStartPosition();

            if (position.Line != referencePosition.Line)
            {
                var spaces = referencePosition.Character + Offset;

                if (position.Character != spaces)
                {
                    var issue = Issue(whereKeyword, CreateProposalForSpaces(spaces));

                    return new[] { issue };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}