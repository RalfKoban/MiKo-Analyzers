﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6053";

        public MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.Argument);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArgumentSyntax argument)
            {
                var issues = AnalyzeNode(argument);

                if (issues.Length > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private Diagnostic[] AnalyzeNode(ArgumentSyntax argument)
        {
            if (argument.Expression is MemberAccessExpressionSyntax maes && maes.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                if (maes.Expression.IsOnSameLineAs(maes.Name) is false)
                {
                    return new[] { Issue(argument) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}