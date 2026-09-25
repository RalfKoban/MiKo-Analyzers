using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3084_YodaExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3084";

        private static readonly SyntaxKind[] ExpressionValues =
                                                                {
                                                                    SyntaxKind.TrueLiteralExpression,
                                                                    SyntaxKind.FalseLiteralExpression,
                                                                    SyntaxKind.NullLiteralExpression,
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    SyntaxKind.StringLiteralExpression,
                                                                };

        private static readonly SyntaxKind[] Expressions =
                                                           {
                                                               SyntaxKind.EqualsExpression,
                                                               SyntaxKind.NotEqualsExpression,
                                                               SyntaxKind.LessThanExpression,
                                                               SyntaxKind.LessThanOrEqualExpression,
                                                               SyntaxKind.GreaterThanExpression,
                                                               SyntaxKind.GreaterThanOrEqualExpression,
                                                           };

        public MiKo_3084_YodaExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeExpression, Expressions);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static bool IsResponsible(CSharpSyntaxNode syntax)
        {
            switch (syntax)
            {
                case null:
                    return false;

                case PrefixUnaryExpressionSyntax u:
                    return u.Operand.IsKind(SyntaxKind.NumericLiteralExpression) && (u.IsKind(SyntaxKind.UnaryPlusExpression) || u.IsKind(SyntaxKind.UnaryMinusExpression));

                default:
                    return syntax.IsAnyKind(ExpressionValues);
            }
        }

        private static bool IsResponsible(CSharpSyntaxNode syntax, in SyntaxNodeAnalysisContext context) => IsResponsible(syntax) || syntax.IsConst(context);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
                var left = node.Left;

                if (IsResponsible(left, context))
                {
                    ReportDiagnostics(context, Issue(left, node.OperatorToken.ValueText));
                }
            }
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax maes)
            {
                var name = maes.GetName();

                if (name is nameof(Equals))
                {
                    var left = maes.Expression;

                    if (IsResponsible(left, context))
                    {
                        ReportDiagnostics(context, Issue(left, name));
                    }
                }
            }
        }
    }
}