using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3083_UsePatternMatchingForNullEqualsExpressionAnalyzer : UsePatternMatchingForBinaryExpressionAnalyzer
    {
        public const string Id = "MiKo_3083";

        public MiKo_3083_UsePatternMatchingForNullEqualsExpressionAnalyzer() : base(Id, SyntaxKind.EqualsExpression)
        {
        }

        protected override bool IsResponsibleNode(SyntaxKind kind) => kind == SyntaxKind.NullLiteralExpression;
    }
}