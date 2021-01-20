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
                SyntaxKind.ElementAccessExpression,
            };

        public MiKo_3030_MethodsFollowLawOfDemeterAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKinds);

        private static bool IsViolation(SyntaxNodeAnalysisContext context, SyntaxNode parent)
        {
            if (parent.Parent is InvocationExpressionSyntax)
            {
                // skip those as it is probably a method invocation
                return false;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;
            switch (symbol)
            {
                case INamespaceOrTypeSymbol _:
                {
                    // probably a nested class or a namespace
                    return false;
                }

                case IPropertySymbol p when Constants.Names.AssertionNamespaces.Contains(p.ContainingNamespace?.FullyQualifiedName()):
                {
                    // skip constraints etc. of NUnit
                    return false;
                }

                default:
                {
                    // TODO: here we could check the parent's grand-parent to get a less-restrict LoD violation (such as 'xyz.Arguments.Length' would then be allowed)
                    return true;
                }
            }
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var parent = node.Parent;

            if (parent != null && SyntaxKinds.Any(_ => _ == parent.Kind()))
            {
                if (IsViolation(context, parent))
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