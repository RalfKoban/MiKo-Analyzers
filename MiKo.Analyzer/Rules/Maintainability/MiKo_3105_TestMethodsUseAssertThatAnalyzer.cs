
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3105_TestMethodsUseAssertThatAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3105";

        public MiKo_3105_TestMethodsUseAssertThatAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;
            var method = context.GetEnclosingMethod();
            if (method == null)
                return;

            if (method.IsTestMethod() || method.ContainingType.IsTestClass())
            {
                if (node.Expression is IdentifierNameSyntax i
                 && i.Identifier.ValueText == "Assert"
                 && node.Name.Identifier.ValueText != "That"
                 && context.SemanticModel.GetTypeInfo(i).Type?.ContainingNamespace.FullyQualifiedName() == "NUnit.Framework")
                {
                    context.ReportDiagnostic(ReportIssue(method.Name, node.Name.GetLocation()));
                }
            }
        }
    }
}