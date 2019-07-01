using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3025_ReuseParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3025";

        public MiKo_3025_ReuseParameterAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);

        private void AnalyzeSimpleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (AssignmentExpressionSyntax)context.Node;

            var diagnostic = AnalyzeSimpleAssignmentExpression(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeSimpleAssignmentExpression(AssignmentExpressionSyntax node, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);

            if (method?.Parameters.Length > 0)
            {
                var names = method.Parameters.Where(_ => _.RefKind == RefKind.None).Select(_ => _.Name).ToHashSet();

                var name = node.Left.ToCleanedUpString();
                if (names.Contains(name))
                {
                    return Issue(name, node.Left.GetLocation());
                }
            }

            return null;
        }
    }
}