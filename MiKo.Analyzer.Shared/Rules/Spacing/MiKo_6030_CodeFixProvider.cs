using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override string Title => Resources.MiKo_6030_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode node, int leadingSpaces)
        {
            // such as for dictionaries
            if (node is InitializerExpressionSyntax initializer
             && initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression) // such as for dictionaries
             && initializer.OpenBraceToken.GetStartingLine() != initializer.CloseBraceToken.GetStartingLine())
            {
                return GetUpdatedSyntax(initializer, leadingSpaces) as TSyntaxNode;
            }

            return base.GetUpdatedSyntax(node, leadingSpaces);
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
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

        private InitializerExpressionSyntax GetUpdatedSyntax(InitializerExpressionSyntax initializer, int spaces) => initializer.WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingSpaces(spaces))
                                                                                                                                .WithExpressions(GetUpdatedSyntax(initializer.Expressions, spaces + Constants.Indentation))
                                                                                                                                .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingSpaces(spaces));

        private AnonymousObjectCreationExpressionSyntax GetUpdatedSyntax(AnonymousObjectCreationExpressionSyntax anonymous, int spaces) => anonymous.WithOpenBraceToken(anonymous.OpenBraceToken.WithLeadingSpaces(spaces))
                                                                                                                                                    .WithInitializers(GetUpdatedSyntax(anonymous.Initializers, spaces + Constants.Indentation))
                                                                                                                                                    .WithCloseBraceToken(anonymous.CloseBraceToken.WithLeadingSpaces(spaces));
    }
}