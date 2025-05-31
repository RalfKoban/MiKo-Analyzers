using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3228_CodeFixProvider)), Shared]
    public sealed class MiKo_3228_CodeFixProvider : UsePatternMatchingCodeFixProvider
    {
        public MiKo_3228_CodeFixProvider() : base(SyntaxKind.NotEqualsExpression)
        {
        }

        public override string FixableDiagnosticId => "MiKo_3228";

        protected override IsPatternExpressionSyntax GetUpdatedPatternSyntax(ExpressionSyntax operand, ExpressionSyntax expression) => UnaryNot(IsPattern(operand, expression));
    }
}