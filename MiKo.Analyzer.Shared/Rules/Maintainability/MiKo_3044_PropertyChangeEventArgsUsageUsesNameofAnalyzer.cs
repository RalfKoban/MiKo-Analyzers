using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3044_PropertyChangeEventArgsUsageUsesNameofAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3044";

        private static readonly SyntaxKind[] EqualsExpressions = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression };

        public MiKo_3044_PropertyChangeEventArgsUsageUsesNameofAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.StringLiteralExpression);

        private static bool HasIssue(LiteralExpressionSyntax literal)
        {
            if (literal.Token.ValueText.IsNullOrWhiteSpace())
            {
                return false;
            }

            switch (literal.Parent)
            {
                case BinaryExpressionSyntax binary when binary.IsAnyKind(EqualsExpressions):
                {
                    return IsPropertyNameAccess(binary.Right == literal ? binary.Left : binary.Right);
                }

                case CaseSwitchLabelSyntax caseLabel:
                {
                    // switch / case
                    return IsPropertyNameAccess(caseLabel.FirstAncestorOrSelf<SwitchStatementSyntax>()?.Expression);
                }

                case ConstantPatternSyntax pattern when pattern.Parent is SwitchExpressionArmSyntax arm:
                {
                    // switch expression in an arm
                    return IsPropertyNameAccess(arm.FirstAncestorOrSelf<SwitchExpressionSyntax>()?.GoverningExpression);
                }

                case MemberAccessExpressionSyntax maes when maes.IsKind(SyntaxKind.SimpleMemberAccessExpression) && maes.Parent is InvocationExpressionSyntax invocation && IsEqualsCall(maes):
                {
                    // seems like a normal Equals() method
                    return IsPropertyNameAccess(invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression);
                }

                case ArgumentSyntax argument when argument.Parent?.Parent is InvocationExpressionSyntax invocation && IsEqualsCall(invocation.Expression):
                {
                    var arguments = invocation.ArgumentList.Arguments;

                    if (arguments.Count == 1)
                    {
                        // seems like a normal Equals() method
                        return IsPropertyName(invocation);
                    }

                    // seems like a static Equals() method
                    var otherArgument = argument.Equals(arguments[0]) ? arguments[1] : arguments[0];

                    return IsPropertyNameAccess(otherArgument.Expression);
                }

                default:
                {
                    return false;
                }
            }
        }

        private static bool IsEqualsCall(ExpressionSyntax expression)
        {
            var name = expression.GetName();

            return name == nameof(Equals);
        }

        private static bool IsPropertyName(ExpressionSyntax expression)
        {
            var name = expression.GetName();

            return name == "PropertyName";
        }

        private static bool IsPropertyNameAccess(ExpressionSyntax expression) => expression is MemberAccessExpressionSyntax syntax && syntax.IsKind(SyntaxKind.SimpleMemberAccessExpression) && IsPropertyName(syntax);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var literal = (LiteralExpressionSyntax)context.Node;

            if (HasIssue(literal))
            {
                ReportDiagnostics(context, Issue(literal));
            }
        }
    }
}