using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3227_UsePatternMatchingForLiteralEqualsExpressionAnalyzer : UsePatternMatchingForBinaryExpressionAnalyzer
    {
        public const string Id = "MiKo_3227";

        public MiKo_3227_UsePatternMatchingForLiteralEqualsExpressionAnalyzer() : base(Id, SyntaxKind.EqualsExpression, LanguageVersion.CSharp9)
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
    }
}