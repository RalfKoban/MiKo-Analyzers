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

            if (symbol.ContainingType.IsTestClass())
            {
                // ignore unit tests
                return;
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
                    case FieldDeclarationSyntax field when field.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                        return null; // ignore enum members

                    case EnumMemberDeclarationSyntax _:
                        return null; // ignore constants

                    case AttributeArgumentSyntax _:
                        return null; // ignore constants

                    case PrefixUnaryExpressionSyntax prefix when prefix.IsKind(SyntaxKind.UnaryMinusExpression) && node.Token.Text == "1":
                        return null; // ignore -1
                }
            }

            switch (node.Parent?.Kind())
            {
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.UnaryPlusExpression:
                {
                    return Issue(symbol.Name, node.Parent);
                }

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
            }

            if (node.Token.Text == "0")
            {
                return null; // ignore zero
            }

            return Issue(symbol.Name, node);
        }
    }
}