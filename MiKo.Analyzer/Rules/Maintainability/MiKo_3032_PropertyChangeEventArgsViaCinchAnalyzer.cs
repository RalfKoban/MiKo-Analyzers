using System;

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

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Expression is IdentifierNameSyntax i && i.Identifier.ValueText.EndsWith("ObservableHelper", StringComparison.Ordinal))
            {
                var methodName = node.Name.Identifier.ValueText;

                if (methodName == "CreateArgs" || methodName == "GetPropertyName")
                {
                    var symbol = node.Parent.GetEnclosingSymbol(context.SemanticModel);
                    var issue = Issue(symbol?.Name, node.Parent.GetLocation());
                    context.ReportDiagnostic(issue);
                }
            }
        }
    }
}