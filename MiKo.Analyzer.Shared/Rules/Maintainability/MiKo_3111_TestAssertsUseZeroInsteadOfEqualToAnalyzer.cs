﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3111_TestAssertsUseZeroInsteadOfEqualToAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3111";

        public MiKo_3111_TestAssertsUseZeroInsteadOfEqualToAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpressionSyntax, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocationExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var issues = Analyze(node);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private Diagnostic[] Analyze(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax maes && maes.Name.GetName() == "EqualTo")
            {
                var argumentList = node.ArgumentList;
                var arguments = argumentList.Arguments;

                if (arguments.Count == 1 && arguments[0].Expression is LiteralExpressionSyntax literal && literal.Token.ValueText == "0")
                {
                    var location = CreateLocation(node, maes.Name.Span.Start, argumentList.Span.End);

                    return new[] { Issue(location) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}