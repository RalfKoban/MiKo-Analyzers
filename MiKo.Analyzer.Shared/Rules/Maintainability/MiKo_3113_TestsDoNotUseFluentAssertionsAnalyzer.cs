using System;

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

        private static readonly Func<SyntaxNode, bool> IsAncestor = IsAncestorCore;

        public MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool IsApplicable(Compilation compilation) => compilation.GetTypeByMetadataName("FluentAssertions.AssertionExtensions") != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpressionStatementSyntax, SyntaxKind.ExpressionStatement);

        private static bool IsAncestorCore(SyntaxNode node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.ExpressionStatement:
                case (int)SyntaxKind.SimpleLambdaExpression:
                case (int)SyntaxKind.ParenthesizedLambdaExpression:
                    return true;

                default:
                    return false;
            }
        }

        private static ExpressionSyntax FindProblematicNode(ExpressionStatementSyntax node)
        {
            var problematicNode = node.GetFluentAssertionShouldNode();
            var root = problematicNode?.FirstAncestor(IsAncestor);

            switch (root)
            {
                case ExpressionStatementSyntax statement: return statement.Expression;
                case LambdaExpressionSyntax lambda: return lambda.ExpressionBody;
                default: return null;
            }
        }

        private void AnalyzeExpressionStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (ExpressionStatementSyntax)context.Node;

            var issues = Analyze(node);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private Diagnostic[] Analyze(ExpressionStatementSyntax node)
        {
            var problematicNode = FindProblematicNode(node);

            return problematicNode != null
                   ? new[] { Issue(problematicNode) }
                   : Array.Empty<Diagnostic>();
        }
    }
}