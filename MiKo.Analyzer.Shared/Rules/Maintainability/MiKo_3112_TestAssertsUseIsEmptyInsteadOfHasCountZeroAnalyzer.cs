using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3112";

        public MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMemberAccessExpressionSyntaxSyntax, SyntaxKind.SimpleMemberAccessExpression);

        private static bool HasIssue(MemberAccessExpressionSyntax node)
        {
            var code = node.ToString();

            switch (code)
            {
                case "Has.Count.Zero":
                case "Has.Not.Count.Zero":
                case "Has.Exactly(0).Items":
                    return true;

                default:
                    return false;
            }
        }

        private void AnalyzeMemberAccessExpressionSyntaxSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            var issues = Analyze(node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> Analyze(MemberAccessExpressionSyntax node) => HasIssue(node)
                                                                                      ? new[] { Issue(node) }
                                                                                      : Enumerable.Empty<Diagnostic>();
    }
}