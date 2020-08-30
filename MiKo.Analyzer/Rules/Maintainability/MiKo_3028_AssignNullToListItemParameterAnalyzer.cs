using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    // see also MiKo_3025_ReuseParameterAnalyzer
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3028_AssignNullToListItemParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3028";

        public MiKo_3028_AssignNullToListItemParameterAnalyzer() : base(Id)
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
            if (node.Right?.IsKind(SyntaxKind.NullLiteralExpression) is true)
            {
                var method = node.GetEnclosingMethod(semanticModel);
                if (method?.Parameters.Length > 0)
                {
                    var names = method.Parameters.Where(_ => _.RefKind == RefKind.None).Select(_ => _.Name).ToHashSet();

                    var name = node.Left.ToCleanedUpString();
                    if (names.Contains(name))
                    {
                        // TODO RKN: Check for ForEach
                        if (node.FirstAncestorOrSelf<SimpleLambdaExpressionSyntax>() != null)
                        {
                            return Issue(name, node);
                        }
                    }
                }
            }

            return null;
        }
    }
}