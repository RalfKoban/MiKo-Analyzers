using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3107";

        public MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeInitializerExpression, SyntaxKind.ObjectInitializerExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInitializerExpression, SyntaxKind.ArrayInitializerExpression);
            context.RegisterSyntaxNodeAction(AnalyzeNormalInvocationExpression, SyntaxKind.InvocationExpression);
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var method = node.GetEnclosingMethod(semanticModel);

            return AnalyzeArguments(method, argumentList);
        }

        private static bool IsConditionMatcher(MemberAccessExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax invokedType && invokedType.GetName() == "It")
            {
                switch (node.GetName())
                {
                    case "Is":
                    case "IsAny":
                    case "IsIn":
                    case "IsInRange":
                    case "IsNotIn":
                    case "IsNotNull":
                    case "IsRegex":
                        return true;
                }
            }

            return false;
        }

        private void AnalyzeNormalInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostics = AnalyzeNormalInvocationExpression(node, context.SemanticModel);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNormalInvocationExpression(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;

            foreach (var lambda in node.AncestorsAndSelf().OfType<SimpleLambdaExpressionSyntax>())
            {
                if (lambda.Parent is ArgumentSyntax a && a.Parent?.Parent is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax m)
                {
                    switch (m.GetName())
                    {
                        case "Setup":
                        case "SetupGet":
                        case "SetupSet":
                        case "SetupSequence":
                        case "Verify":
                        case "VerifyGet":
                        case "VerifySet":
                        {
                            // here we assume that we have a Moq call
                            return Enumerable.Empty<Diagnostic>();
                        }
                    }
                }
            }

            var method = node.GetEnclosingMethod(semanticModel);

            return AnalyzeArguments(method, argumentList);
        }

        private void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (InitializerExpressionSyntax)context.Node;

            var diagnostics = AnalyzeInitializerExpression(node, context.SemanticModel);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeInitializerExpression(InitializerExpressionSyntax node, SemanticModel semanticModel)
        {
            var symbol = node.GetEnclosingSymbol(semanticModel);

            var diagnostics = node.Expressions.OfType<AssignmentExpressionSyntax>()
                                                         .Where(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression))
                                                         .SelectMany(_ => AnalyzeExpression(_.Right, symbol.Name, _.GetLocation()));
            return diagnostics;
        }

        private IEnumerable<Diagnostic> AnalyzeArguments(IMethodSymbol method, ArgumentListSyntax argumentList)
        {
            if (method is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var diagnostics = argumentList.Arguments.SelectMany(_ => AnalyzeExpression(_.Expression, method.Name, _.GetLocation()));
            return diagnostics;
        }

        private IEnumerable<Diagnostic> AnalyzeExpression(ExpressionSyntax expression, string methodName, Location location)
        {
            var hasIssue = expression is InvocationExpressionSyntax ies
                        && ies.Expression is MemberAccessExpressionSyntax mae
                        && mae.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                        && IsConditionMatcher(mae);

            if (hasIssue)
            {
                yield return Issue(methodName, location);
            }
        }
    }
}