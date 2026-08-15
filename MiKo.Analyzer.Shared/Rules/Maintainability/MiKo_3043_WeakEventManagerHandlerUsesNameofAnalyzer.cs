using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3043";

        public MiKo_3043_WeakEventManagerHandlerUsesNameofAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);

        private Diagnostic[] AnalyzeIssue(InvocationExpressionSyntax invocation, ISymbol method)
        {
            if (invocation.Is("WeakEventManager", "AddHandler") || invocation.Is("WeakEventManager", "RemoveHandler"))
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (arguments.Count >= 2)
                {
                    var argument = arguments[1];

                    if (argument.Expression.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        return new[] { Issue(method.Name, argument) };
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var methodSymbol = context.GetEnclosingMethod();

            if (methodSymbol is null)
            {
                // nameof() is also a InvocationExpressionSyntax, so assignments of lists etc. may cause an NRE to be thrown
                return;
            }

            var node = (InvocationExpressionSyntax)context.Node;
            var issues = AnalyzeIssue(node, methodSymbol);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }
    }
}