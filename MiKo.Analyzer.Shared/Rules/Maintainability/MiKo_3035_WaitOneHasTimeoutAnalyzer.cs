﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3035_WaitOneHasTimeoutAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3035";

        public MiKo_3035_WaitOneHasTimeoutAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool NodeHasIssue(MemberAccessExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.GetName() != "WaitOne")
            {
                return false;
            }

            if (node.Parent is InvocationExpressionSyntax i)
            {
                var arguments = i.ArgumentList.Arguments;
                var argumentsCount = arguments.Count;

                if (argumentsCount > 0)
                {
                    for (var index = 0; index < argumentsCount; index++)
                    {
                        var argumentType = arguments[index].GetTypeSymbol(semanticModel);

                        if (argumentType.SpecialType == SpecialType.System_Int32 || argumentType.FullyQualifiedName() == TypeNames.TimeSpan)
                        {
                            // we use a timeout parameter (int or TimeSpan)
                            return false;
                        }
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
                var methodSymbol = context.GetEnclosingMethod();

                if (methodSymbol is null)
                {
                    // nameof() is also a SimpleMemberAccessExpression, so assignments of lists etc. may cause an NRE to be thrown
                    return;
                }

                ReportDiagnostics(context, Issue(methodSymbol.Name, node));
            }
        }
    }
}