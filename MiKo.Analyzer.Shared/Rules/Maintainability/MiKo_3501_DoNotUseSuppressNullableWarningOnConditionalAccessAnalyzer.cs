using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer : DoNotUseSuppressNullableWarningAnalyzer
    {
        public const string Id = "MiKo_3501";

        public MiKo_3501_DoNotUseSuppressNullableWarningOnConditionalAccessAnalyzer() : base(Id)
        {
        }

        protected override bool HasIssue(PostfixUnaryExpressionSyntax warningExpression) => warningExpression.Operand is ConditionalAccessExpressionSyntax;
    }
}