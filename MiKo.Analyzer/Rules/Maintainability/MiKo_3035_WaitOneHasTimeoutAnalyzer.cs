using System;

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

        public MiKo_3035_WaitOneHasTimeoutAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;
            if (node.Name.Identifier.ValueText != "WaitOne")
                return;

            if (node.Parent is InvocationExpressionSyntax i)
            {
                foreach (var argument in i.ArgumentList.Arguments)
                {
                    var argumentType = argument.Expression.GetTypeSymbol(context.SemanticModel);

                    if (argumentType.SpecialType == SpecialType.System_Int32 || argumentType.FullyQualifiedName() == typeof(TimeSpan).FullName)
                    {
                        // we use a timeout parameter (int or TimeSpan)
                        return;
                    }
                }
            }

            var method = context.GetEnclosingMethod();
            context.ReportDiagnostic(Issue(method.Name, node.GetLocation()));
        }
    }
}