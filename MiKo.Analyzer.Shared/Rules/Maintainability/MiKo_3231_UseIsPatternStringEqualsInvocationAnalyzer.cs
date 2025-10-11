using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3231_UseIsPatternStringEqualsInvocationAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3231";

        public MiKo_3231_UseIsPatternStringEqualsInvocationAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static bool IsStringComparisonOrdinal(ExpressionSyntax expression) => expression is MemberAccessExpressionSyntax m
                                                                                   && m.GetIdentifierName() is nameof(StringComparison)
                                                                                   && m.GetName() is nameof(StringComparison.Ordinal);

        private static bool IsStringComparisonOrdinal(in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            switch (arguments.Count)
            {
                case 1:
                case 2 when IsStringComparisonOrdinal(arguments[1].Expression):
                    return true;

                default:
                    return false;
            }
        }

        private static bool HasIssue(InvocationExpressionSyntax invocation, in SyntaxNodeAnalysisContext context)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax maes && maes.GetName() is nameof(Equals))
            {
                var arguments = invocation.ArgumentList.Arguments;

                if (IsStringComparisonOrdinal(arguments))
                {
                    var argumentExpression = arguments[0].Expression;
                    var expression = maes.Expression;

                    if (argumentExpression.IsKind(SyntaxKind.StringLiteralExpression) || expression.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        return true;
                    }

                    if (argumentExpression.IsNameOf())
                    {
                        return true;
                    }

                    if (expression.IsNameOf())
                    {
                        return true;
                    }

                    if (argumentExpression.IsConst(context) || expression.IsConst(context))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation && HasIssue(invocation, context))
            {
                ReportDiagnostics(context, Issue(invocation));
            }
        }
    }
}