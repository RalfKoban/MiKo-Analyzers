﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3061_LoggerHasCategoryAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3061";

        public MiKo_3061_LoggerHasCategoryAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsLogManagerGetLoggerCall(MemberAccessExpressionSyntax node) => node.GetName() == "GetLogger"
                                                                                         && node.Expression is IdentifierNameSyntax i
                                                                                         && i.GetName().EndsWith("LogManager", StringComparison.Ordinal);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Parent is InvocationExpressionSyntax s && s.ArgumentList.Arguments.Count == 1 && IsLogManagerGetLoggerCall(node))
            {
                var argument = s.ArgumentList.Arguments[0];

                var type = argument.GetTypeSymbol(context.SemanticModel);

                if (type.SpecialType != SpecialType.System_String)
                {
                    var issue = Issue(context.ContainingSymbol?.Name, argument);

                    context.ReportDiagnostic(issue);
                }
            }
        }
    }
}