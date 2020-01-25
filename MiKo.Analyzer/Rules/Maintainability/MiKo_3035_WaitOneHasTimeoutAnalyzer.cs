﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3035_WaitOneHasTimeoutAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3035";

        private static readonly string TimeSpanFullName = typeof(TimeSpan).FullName;

        public MiKo_3035_WaitOneHasTimeoutAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool NodeHasIssue(MemberAccessExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.GetName() != "WaitOne")
            {
                return false;
            }

            if (node.Parent is InvocationExpressionSyntax i)
            {
                foreach (var argument in i.ArgumentList.Arguments)
                {
                    var argumentType = argument.Expression.GetTypeSymbol(semanticModel);

                    if (argumentType.SpecialType == SpecialType.System_Int32 || argumentType.FullyQualifiedName() == TimeSpanFullName)
                    {
                        // we use a timeout parameter (int or TimeSpan)
                        return false;
                    }
                }
            }

            return true;
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (NodeHasIssue(node, context.SemanticModel))
            {
                var method = context.GetEnclosingMethod();
                var issue = Issue(method.Name, node.GetLocation());

                context.ReportDiagnostic(issue);
            }
        }
    }
}