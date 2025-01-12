﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc/>
    /// <seealso cref="MiKo_3120_UseValueInsteadOfItIsConditionMatcherAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3107";

        public MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.Moq.MockFullQualified) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
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
                return Array.Empty<Diagnostic>();
            }

            var method = node.GetEnclosingMethod(semanticModel);

            return AnalyzeArguments(method, argumentList);
        }

        private static bool IsConditionMatcher(MemberAccessExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax invokedType && invokedType.GetName() == Constants.Moq.ConditionMatcher.It)
            {
                switch (node.GetName())
                {
                    case Constants.Moq.ConditionMatcher.Is:
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
            var issues = AnalyzeNormalInvocationExpression(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeNormalInvocationExpression(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;

            if (node.Ancestors<LambdaExpressionSyntax>().Any(_ => _.IsMoqCall()))
            {
                return Array.Empty<Diagnostic>();
            }

            var method = node.GetEnclosingMethod(semanticModel);

            return AnalyzeArguments(method, argumentList);
        }

        private void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (InitializerExpressionSyntax)context.Node;
            var issues = AnalyzeInitializerExpression(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeInitializerExpression(InitializerExpressionSyntax node, SemanticModel semanticModel)
        {
            var symbol = node.GetEnclosingSymbol(semanticModel);
            var symbolName = symbol?.Name;

            foreach (var expression in node.Expressions)
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.SimpleAssignmentExpression:
                    {
                        foreach (var issue in AnalyzeExpression(((AssignmentExpressionSyntax)expression).Right, symbolName, expression.GetLocation()))
                        {
                            yield return issue;
                        }

                        break;
                    }

                    case SyntaxKind.InvocationExpression:
                    {
                        foreach (var issue in AnalyzeExpression(expression, symbolName, expression.GetLocation()))
                        {
                            yield return issue;
                        }

                        break;
                    }

                    case SyntaxKind.ObjectCreationExpression:
                    {
                        var argumentList = ((ObjectCreationExpressionSyntax)expression).ArgumentList;

                        if (argumentList != null)
                        {
                            foreach (var issue in AnalyzeArguments(symbolName, argumentList))
                            {
                                yield return issue;
                            }
                        }

                        break;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeArguments(IMethodSymbol method, ArgumentListSyntax argumentList) => method is null
                                                                                                                   ? Array.Empty<Diagnostic>()
                                                                                                                   : AnalyzeArguments(method.Name, argumentList);

        private IEnumerable<Diagnostic> AnalyzeArguments(string name, ArgumentListSyntax argumentList) => argumentList.Arguments.SelectMany(_ => AnalyzeExpression(_.Expression, name, _.GetLocation()));

        private Diagnostic[] AnalyzeExpression(ExpressionSyntax expression, string methodName, Location location)
        {
            var hasIssue = expression is InvocationExpressionSyntax ies
                        && ies.Expression is MemberAccessExpressionSyntax mae
                        && mae.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                        && IsConditionMatcher(mae);

            if (hasIssue)
            {
                return new[] { Issue(methodName, location) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}