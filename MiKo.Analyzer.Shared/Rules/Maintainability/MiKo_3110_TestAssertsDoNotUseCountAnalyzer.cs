﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3110_TestAssertsDoNotUseCountAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3110";

        private static readonly HashSet<string> AssertionMethods = new HashSet<string>
                                                                       {
                                                                           "That",
                                                                           "AreEqual",
                                                                           "AreNotEqual",
                                                                           "AreSame",
                                                                           "AreNotSame",
                                                                           "Less",
                                                                           "LessOrEqual",
                                                                           "Greater",
                                                                           "GreaterOrEqual",
                                                                       };

        public MiKo_3110_TestAssertsDoNotUseCountAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => AssertionMethods.Contains(node.GetName())
                                                                                    && node.Expression is IdentifierNameSyntax invokedType
                                                                                    && Constants.Names.AssertionTypes.Contains(invokedType.GetName());

        private static bool IsFixableAssertionForLinqCall(InvocationExpressionSyntax invocation)
        {
            if (invocation.GetIdentifierName() is "Is")
            {
                switch (invocation.GetName())
                {
                    case "EqualTo":
                    case "Zero":
                        return true;
                }
            }

            return false;
        }

        private static bool HasIssue(MemberAccessExpressionSyntax expression, out SyntaxToken token)
        {
            switch (expression.GetName())
            {
                case "Count":
                case "Length":
                {
                    token = expression.Name.Identifier;

                    return true;
                }
            }

            token = default;

            return false;
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberAccessExpressionSyntax maes)
            {
                var issues = AnalyzeSimpleMemberAccessExpression(maes);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (IsAssertionMethod(node) && node.Parent is InvocationExpressionSyntax methodCall)
            {
                var arguments = methodCall.ArgumentList.Arguments;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                for (int index = 0, argumentsCount = arguments.Count; index < argumentsCount; index++)
                {
                    var issue = AnalyzeArgument(node, arguments[index], arguments);

                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private Diagnostic AnalyzeArgument(MemberAccessExpressionSyntax node, ArgumentSyntax argument, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            switch (argument.Expression)
            {
                case MemberAccessExpressionSyntax m when HasIssue(m, out var token):
                {
                    // 'values.Count' or 'values.Length' call
                    return Issue(token);
                }

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax m && HasIssue(m, out var token):
                {
                    // linq call
                    switch (node.GetName())
                    {
                        case "AreEqual":
                        {
                            return Issue(token);
                        }

                        case "That" when arguments.Count >= 2:
                        {
                            var expression = arguments[1].Expression;

                            switch (expression)
                            {
                                case InvocationExpressionSyntax ai when IsFixableAssertionForLinqCall(ai):
                                    // we can only fix "Assert.That(xyz.Count(), Is.EqualTo(42)"
                                    return Issue(token);

                                case MemberAccessExpressionSyntax am when am.GetName() is "Zero":
                                    // we can only fix "Assert.That(xyz.Count(), Is.Zero"
                                    return Issue(token);
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            return null;
        }

        private new Diagnostic Issue(in SyntaxToken token) => Issue(token, new Pair(Constants.AnalyzerCodeFixSharedData.Marker, token.ValueText));
    }
}