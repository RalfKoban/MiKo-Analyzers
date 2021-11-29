using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3038_DoNotUseMagicNumbersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3038";

        public MiKo_3038_DoNotUseMagicNumbersAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNumericLiteralExpression, SyntaxKind.NumericLiteralExpression);

        private static bool IgnoreBasedOnAncestor(LiteralExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case LocalDeclarationStatementSyntax variable when variable.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                    case FieldDeclarationSyntax field when field.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                    case EnumMemberDeclarationSyntax _:
                    case AttributeArgumentSyntax _:
                        return true;
                }
            }

            return false;
        }

        private static bool IgnoreBasedOnParent(LiteralExpressionSyntax node)
        {
            switch (node.Parent?.Kind())
            {
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    return false;

                case SyntaxKind.CaseSwitchLabel:
                case SyntaxKind.RangeExpression:
                    return true;

                case SyntaxKind.Argument:
                    return IgnoreBasedOnArgument(node.Parent); // we want to know what those numbers mean

                case SyntaxKind.SimpleAssignmentExpression:
                    return false; // assignments to width and height (???)

                default:
                    return false;
            }
        }

        private static bool IgnoreBasedOnArgument(SyntaxNode node)
        {
            if (node.Parent is ArgumentListSyntax list && list.Parent is ObjectCreationExpressionSyntax o)
            {
                var name = o.Type.GetNameOnlyPart();
                if (name == nameof(DateTime))
                {
                    const int MinimumArgumentsForHoursMinutesSeconds = 3;

                    if (list.Arguments.Count >= MinimumArgumentsForHoursMinutesSeconds)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsWellknownNumber(string number)
        {
            switch (number.ToLowerCase())
            {
                // ignore zero
                case "0":
                case "0l":
                case "0u":
                case "0d":
                case "0f":
                case "0.0":
                case "0.0f":
                case "0.0d":

                // ignore one as it is often used as offset
                case "1":
                case "1l":
                case "1u":
                case "1d":
                case "1f":
                case "1.0":
                case "1.0f":
                case "1.0d":
                    return true;
            }

            return false;
        }

        private void AnalyzeNumericLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (LiteralExpressionSyntax)context.Node;

            var symbol = context.ContainingSymbol;
            if (symbol is null)
            {
                return;
            }

            if (symbol is IMethodSymbol method)
            {
                if (method.Name == nameof(GetHashCode))
                {
                    // ignore hash calculation
                    return;
                }

                if (method.IsTestMethod())
                {
                    // ignore unit tests
                    return;
                }
            }

            var containingType = symbol.ContainingType;
            if (containingType != null)
            {
                if (containingType.IsTestClass())
                {
                    // ignore unit tests
                    return;
                }

                if (containingType.ContainingType?.IsTestClass() is true)
                {
                    // ignore nested types in unit tests
                    return;
                }
            }

            var number = node.Token.Text;
            if (IsWellknownNumber(number))
            {
                return;
            }

            if (IgnoreBasedOnParent(node) || IgnoreBasedOnAncestor(node))
            {
                return;
            }

            context.ReportDiagnostic(Issue(symbol.Name, node, number));
        }
    }
}