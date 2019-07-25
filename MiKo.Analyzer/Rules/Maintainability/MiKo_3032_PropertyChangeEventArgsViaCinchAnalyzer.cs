using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3032";

        public MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static string GetPropertyName(SyntaxNode node)
        {
            var lambda = node.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>().FirstOrDefault();
            var property = lambda?.Body as MemberAccessExpressionSyntax;
            var propertyName = property?.Name.Identifier.ValueText ?? string.Empty;
            return propertyName;
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Expression is IdentifierNameSyntax i && i.Identifier.ValueText.EndsWith("ObservableHelper", StringComparison.Ordinal))
            {
                switch (node.Name.Identifier.ValueText)
                {
                    case "CreateArgs":
                        ReportIssue(context, node.Parent, "new PropertyChangedEventArgs(nameof({0}))");
                        break;

                    case "GetPropertyName":
                        ReportIssue(context, node.Parent, "nameof({0})");
                        break;
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode node, string proposalFormat)
        {
            var propertyName = GetPropertyName(node);
            var symbol = node.GetEnclosingSymbol(context.SemanticModel);

            var issue = Issue(symbol?.Name, node.GetLocation(), string.Format(proposalFormat, propertyName));
            context.ReportDiagnostic(issue);
        }
    }
}