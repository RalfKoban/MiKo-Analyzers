using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3228_UsePatternMatchingForLiteralNotEqualsExpressionAnalyzer : UsePatternMatchingForBinaryExpressionAnalyzer
    {
        public const string Id = "MiKo_3228";

        public MiKo_3228_UsePatternMatchingForLiteralNotEqualsExpressionAnalyzer() : base(Id, SyntaxKind.NotEqualsExpression, LanguageVersion.CSharp9)
        {
        }

        protected override bool IsResponsibleNode(in SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                    return true;

                default:
                    return false;
            }
        }

        protected override bool IsResponsibleNode(ExpressionSyntax node, SemanticModel semanticModel)
        {
            switch (node)
            {
                case LiteralExpressionSyntax literal: return IsResponsibleNode(literal.Kind());
                case PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.UnaryMinusExpression): return IsResponsibleNode(unary.Operand.Kind());
                case MemberAccessExpressionSyntax maes: return maes.IsEnum(semanticModel);
                default:
                    return false;
            }
        }
    }
}