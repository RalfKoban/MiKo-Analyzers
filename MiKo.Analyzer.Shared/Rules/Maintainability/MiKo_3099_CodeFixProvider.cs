using System.Composition;
using System.Threading;
using System.Threading.Tasks;

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

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static LiteralExpressionSyntax GetUpdatedSyntax(SyntaxNode syntax)
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