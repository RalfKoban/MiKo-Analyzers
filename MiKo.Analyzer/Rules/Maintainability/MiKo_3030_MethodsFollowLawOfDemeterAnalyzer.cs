using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3030_MethodsFollowLawOfDemeterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3030";

        private static readonly SyntaxKind[] SyntaxKinds =
            {
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.ConditionalAccessExpression,
            };

        public MiKo_3030_MethodsFollowLawOfDemeterAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKinds);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var contextNode = context.Node;
            var parent = contextNode.Parent;

            // TODO: what about if/switch
            if (parent != null && SyntaxKinds.Any(_ => _ == parent.Kind()))
            {
                if (parent.Parent is InvocationExpressionSyntax)
                {
                    // skip those as it is probably a method invocation
                }
                else
                {
                    ReportIssue(context, parent);
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var issue = Issue(string.Empty, node);

            context.ReportDiagnostic(issue);
        }
    }
}