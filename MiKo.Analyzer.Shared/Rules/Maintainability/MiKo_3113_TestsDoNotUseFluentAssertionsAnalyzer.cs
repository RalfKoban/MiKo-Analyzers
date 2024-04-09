using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3113";

        private static readonly SyntaxKind[] Ancestors = { SyntaxKind.ExpressionStatement, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression };

        public MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName("FluentAssertions.AssertionExtensions") != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpressionStatementSyntax, SyntaxKind.ExpressionStatement);

        private void AnalyzeExpressionStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (ExpressionStatementSyntax)context.Node;

            var issues = Analyze(node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> Analyze(ExpressionStatementSyntax node)
        {
            var problematicNode = node.GetFluentAssertionShouldNode();

            if (problematicNode != null)
            {
                var root = problematicNode.FirstAncestor<SyntaxNode>(_ => _.IsAnyKind(Ancestors));

                if (root is ExpressionStatementSyntax statement)
                {
                    yield return Issue(statement.Expression);
                }
                else if (root is LambdaExpressionSyntax lambda)
                {
                    yield return Issue(lambda.ExpressionBody);
                }
            }
        }
    }
}