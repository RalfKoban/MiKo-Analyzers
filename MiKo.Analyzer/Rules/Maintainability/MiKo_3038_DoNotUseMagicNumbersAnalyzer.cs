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

        private void AnalyzeNumericLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (LiteralExpressionSyntax)context.Node;

            var symbol = context.ContainingSymbol;
            if (symbol is null)
            {
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

            if (symbol is IMethodSymbol method && method.IsTestMethod())
            {
                // ignore unit tests
                return;
            }

            var issue = FindIssue(node, symbol);
            if (issue != null)
            {
                context.ReportDiagnostic(issue);
            }
        }

        private Diagnostic FindIssue(LiteralExpressionSyntax node, ISymbol symbol)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case LocalDeclarationStatementSyntax variable when variable.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                        return null; // ignore const variables

                    case FieldDeclarationSyntax field when field.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                        return null; // ignore const fields

                    case EnumMemberDeclarationSyntax _:
                        return null; // ignore enum members

                    case AttributeArgumentSyntax _:
                        return null; // ignore constants in attributes
                }
            }

            switch (node.Parent?.Kind())
            {
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                {
                    if (node.Token.Text == "1")
                    {
                        return null; // ignore 1
                    }

                    break;
                }

                case SyntaxKind.CaseSwitchLabel:
                {
                    return null; // ignore case statements
                }
            }

            if (node.Token.Text == "0")
            {
                return null; // ignore zero
            }

            return Issue(symbol.Name, node);
        }
    }
}