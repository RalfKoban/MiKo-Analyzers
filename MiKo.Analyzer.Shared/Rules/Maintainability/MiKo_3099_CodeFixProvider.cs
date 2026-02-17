using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3099_CodeFixProvider)), Shared]
    public sealed class MiKo_3099_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3099";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary:
                    return binary.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken)
                           ? FalseLiteral()
                           : TrueLiteral();

                case IsPatternExpressionSyntax pattern:
                    return pattern.Pattern is UnaryPatternSyntax
                           ? TrueLiteral()
                           : FalseLiteral();

                default:
                    return null;
            }
        }
    }
}