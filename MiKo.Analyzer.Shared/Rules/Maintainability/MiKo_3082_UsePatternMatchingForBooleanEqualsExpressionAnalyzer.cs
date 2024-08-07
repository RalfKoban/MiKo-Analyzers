using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer : UsePatternMatchingForBinaryExpressionAnalyzer
    {
        public const string Id = "MiKo_3082";

        public MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer() : base(Id, SyntaxKind.EqualsExpression)
        {
        }

        protected override bool IsResponsibleNode(SyntaxKind kind) => kind == SyntaxKind.TrueLiteralExpression || kind == SyntaxKind.FalseLiteralExpression;
    }
}