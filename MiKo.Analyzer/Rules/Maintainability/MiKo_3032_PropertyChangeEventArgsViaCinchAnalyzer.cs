using System;
using System.Collections.Generic;
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

        internal const string GetPropertyName = "GetPropertyName"; // use nameof()
        internal const string CreateArgs = "CreateArgs"; // use new PropertyChanged(nameof())

        public MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer() : base(Id)
        {
        }

        internal static string FindPropertyName(SyntaxNode node)
        {
            var lambda = node.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>().FirstOrDefault();
            var property = lambda?.Body as MemberAccessExpressionSyntax;
            var propertyName = property.GetName() ?? string.Empty;
            return propertyName;
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Expression is IdentifierNameSyntax i && i.GetName().EndsWith("ObservableHelper", StringComparison.Ordinal))
            {
                var name = node.GetName();
                switch (name)
                {
                    case GetPropertyName:
                    case CreateArgs:
                        ReportIssue(context, node.Parent, name);
                        break;
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode node, string issueInfo)
        {
            var propertyName = FindPropertyName(node);
            var symbol = node.GetEnclosingSymbol(context.SemanticModel);

            var issue = Issue(symbol?.Name, node, propertyName, issueInfo);
            if (issue != null)
            {
                context.ReportDiagnostic(issue);
            }
        }

        private Diagnostic Issue(string symbolName, SyntaxNode node, string propertyName, string issueInfo)
        {
            // TODO: RKN Fix performance issue on string.Format
            switch (issueInfo)
            {
                case GetPropertyName:
                    return Issue(symbolName, node, $"nameof({propertyName})", new Dictionary<string, string> { { GetPropertyName, string.Empty } });

                case CreateArgs:
                    return Issue(symbolName, node, $"new PropertyChangedEventArgs(nameof({propertyName}))", new Dictionary<string, string> { { CreateArgs, string.Empty } });

                default:
                    return null;
            }
        }
    }
}