using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3088_CodeFixProvider)), Shared]
    public sealed class MiKo_3088_CodeFixProvider : UsePatternMatchingCodeFixProvider
    {
        public MiKo_3088_CodeFixProvider() : base(SyntaxKind.NotEqualsExpression)
        {
        }

        public override string FixableDiagnosticId => "MiKo_3088";

        protected override string Title => Resources.MiKo_3088_CodeFixTitle;

        protected override IsPatternExpressionSyntax GetUpdatedPatternSyntax(ExpressionSyntax operand, LiteralExpressionSyntax literal) => UnaryNot(IsPattern(operand, literal));
    }
}