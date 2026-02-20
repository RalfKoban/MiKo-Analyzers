using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6030_CodeFixProvider)), Shared]
    public sealed class MiKo_6030_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6030";

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode node, in int leadingSpaces)
        {
            switch (node)
            {
                case InitializerExpressionSyntax initializer when initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression) // such as for dictionaries
                                                               && initializer.OpenBraceToken.IsOnSameLineAs(initializer.CloseBraceToken) is false:
                    return GetUpdatedSyntax(initializer, leadingSpaces) as TSyntaxNode;

                case ImplicitObjectCreationExpressionSyntax creation:
                    return GetUpdatedSyntax(creation, leadingSpaces) as TSyntaxNode;

                case ObjectCreationExpressionSyntax creation:
                    return GetUpdatedSyntax(creation, leadingSpaces) as TSyntaxNode;

                default:
                    return base.GetUpdatedSyntax(node, leadingSpaces);
            }
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            switch (syntax)
            {
                case InitializerExpressionSyntax initializer:
                    return GetUpdatedSyntax(initializer, spaces);

                case AnonymousObjectCreationExpressionSyntax anonymous:
                    return GetUpdatedSyntax(anonymous, spaces);

                default:
                    return syntax;
            }
        }
    }
}