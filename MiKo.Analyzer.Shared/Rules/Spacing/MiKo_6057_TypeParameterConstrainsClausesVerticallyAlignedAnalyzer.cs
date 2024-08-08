﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6057_TypeParameterConstrainsClausesVerticallyAlignedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6057";

        private static readonly SyntaxList<TypeParameterConstraintClauseSyntax> Empty = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();

        public MiKo_6057_TypeParameterConstrainsClausesVerticallyAlignedAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeTypeParameterConstraintClause, SyntaxKind.TypeParameterConstraintClause);

        private static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(SyntaxNode node)
        {
            switch (node)
            {
                case ClassDeclarationSyntax c: return c.ConstraintClauses;
                case InterfaceDeclarationSyntax i: return i.ConstraintClauses;
                case RecordDeclarationSyntax r: return r.ConstraintClauses;
                case StructDeclarationSyntax s: return s.ConstraintClauses;
                case MethodDeclarationSyntax b: return b.ConstraintClauses;

                default:
                    return Empty;
            }
        }

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
            var clauses = GetConstraintClauses(node.Parent);

            if (clauses.Count <= 1)
            {
                // do not report the single one
                yield break;
            }

            var reference = clauses[0];

            if (node == reference)
            {
                // do not report for the first one
                yield break;
            }

            var whereKeyword = node.WhereKeyword;

            var referencePosition = reference.GetPositionWithinStartLine();
            var position = whereKeyword.GetPositionWithinStartLine();

            if (position != referencePosition || whereKeyword.LeadingTrivia.Any(SyntaxKind.EndOfLineTrivia))
            {
                yield return Issue(whereKeyword, CreateProposalForSpaces(referencePosition));
            }
        }
    }
}