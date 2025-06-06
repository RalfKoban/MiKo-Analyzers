﻿using System;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Expression is IdentifierNameSyntax i && i.GetName().EndsWith("ObservableHelper", StringComparison.Ordinal))
            {
                var name = node.GetName();

                switch (name)
                {
                    case Constants.AnalyzerCodeFixSharedData.GetPropertyName:
                        ReportIssue(context, node.Parent, name, "nameof({0})");

                        break;

                    case Constants.AnalyzerCodeFixSharedData.CreateArgs:
                        ReportIssue(context, node.Parent, name, "new PropertyChangedEventArgs(nameof({0}))");

                        break;
                }
            }
        }

        private void ReportIssue(in SyntaxNodeAnalysisContext context, SyntaxNode node, string issueId, string issueTemplate)
        {
            var descendantNodes = node.DescendantNodes().ToList();

            var lambda = descendantNodes.OfType<SimpleLambdaExpressionSyntax>().FirstOrDefault();

            if (lambda?.Body is MemberAccessExpressionSyntax property)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var propertyName = property.GetName();

                var semanticModel = context.SemanticModel;

                var t = descendantNodes.OfType<TypeArgumentListSyntax>().First();
                var type = t.Arguments.First();
                var propertyType = type.GetTypeSymbol(semanticModel);

                var symbol = node.GetEnclosingSymbol(semanticModel);

                // TODO: RKN return type as well "A.PropertyName" if the containing type does not inherit from the type or implement the interface
                var properties = symbol.ContainingType.IsRelated(propertyType)
                                 ? new[] { new Pair(issueId, string.Empty), new Pair(Constants.AnalyzerCodeFixSharedData.PropertyName, propertyName) }
                                 : new[] { new Pair(issueId, string.Empty), new Pair(Constants.AnalyzerCodeFixSharedData.PropertyName, propertyName), new Pair(Constants.AnalyzerCodeFixSharedData.PropertyTypeName, type.GetNameOnlyPart()) };

                ReportDiagnostics(context, Issue(symbol.Name, node, issueTemplate.FormatWith(propertyName), properties));
            }
        }
    }
}