using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3088_UsePatternMatchingForNullNotEqualsExpressionAnalyzer : UsePatternMatchingForBinaryExpressionAnalyzer
    {
        public const string Id = "MiKo_3088";

        public MiKo_3088_UsePatternMatchingForNullNotEqualsExpressionAnalyzer() : base(Id, SyntaxKind.NotEqualsExpression, LanguageVersion.CSharp9)
        {
        }

        protected override bool IsResponsibleNode(SyntaxKind kind) => kind == SyntaxKind.NullLiteralExpression;
    }
}