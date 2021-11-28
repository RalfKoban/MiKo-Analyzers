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

                    case ArgumentSyntax _:
                        return false; // we want to know what those numbers mean
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

                default:
                    return false;
            }
        }

        private void AnalyzeNumericLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (LiteralExpressionSyntax)context.Node;

            var symbol = context.ContainingSymbol;
            if (symbol is null)
            {
                return;
            }

            if (symbol is IMethodSymbol method && method.IsTestMethod())
            {
                // ignore unit tests
                return;
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

            switch (node.Token.Text)
            {
                case "0": // ignore zero
                case "1": // ignore one as it is often used as offset
                    return;
            }

            if (IgnoreBasedOnParent(node) || IgnoreBasedOnAncestor(node))
            {
                return;
            }

            context.ReportDiagnostic(Issue(symbol.Name, node));
        }
    }
}