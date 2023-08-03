﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6041_AssignmentsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6041";

        public MiKo_6041_AssignmentsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EqualsValueClause);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (EqualsValueClauseSyntax)context.Node;

            var startLine = node.EqualsToken.GetStartingLine();
            var expressionLine = node.Value.GetStartingLine();

            if (startLine != expressionLine)
            {
                ReportIssue(context, node);
            }
            else
            {
                var sibling = node.PreviousSiblingNodeOrToken();
                var siblingStartLine = sibling.GetStartingLine();

                if (startLine != siblingStartLine)
                {
                    ReportIssue(context, node);
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, EqualsValueClauseSyntax node)
        {
            if (node.Value is InitializerExpressionSyntax initializer && initializer.OpenBraceToken.GetStartingLine() != initializer.CloseBraceToken.GetStartingLine())
            {
                // arrays and collections spanning multiple lines are allowed
            }
            else
            {
                ReportDiagnostics(context, Issue(node.EqualsToken));
            }
        }
    }
}