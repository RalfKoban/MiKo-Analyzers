using System.Collections.Generic;

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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private IEnumerable<Diagnostic> AnalyzeIssue(MemberAccessExpressionSyntax node, ISymbol method)
        {
            var name = node.GetName();
            switch (name)
            {
                case "AddHandler":
                case "RemoveHandler":
                {
                    if (node.Parent is InvocationExpressionSyntax i)
                    {
                        var typeName = i.GetName();

                        if (typeName == "WeakEventManager")
                        {
                            var arguments = i.ArgumentList.Arguments;

                            if (arguments.Count >= 2)
                            {
                                var argument = arguments[1];

                                if (argument.Expression.IsKind(SyntaxKind.StringLiteralExpression))
                                {
                                    yield return Issue(method.Name, argument);
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            var methodSymbol = context.GetEnclosingMethod();

            if (methodSymbol is null)
            {
                // nameof() is also a SimpleMemberAccessExpression, so assignments of lists etc. may cause an NRE to be thrown
                return;
            }

            ReportDiagnostics(context, AnalyzeIssue(node, methodSymbol));
        }
    }
}