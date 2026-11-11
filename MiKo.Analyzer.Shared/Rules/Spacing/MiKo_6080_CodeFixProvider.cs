using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6080_CodeFixProvider)), Shared]
    public sealed class MiKo_6080_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6080";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            if (syntax is BlockSyntax node)
            {
                var openBrace = node.OpenBraceToken;

                var previousToken = openBrace.GetPreviousToken();

                if (openBrace.IsOnSameLineAsEndOf(previousToken))
                {
                    var updatedRoot = root.ReplaceToken(previousToken, previousToken.WithForcedEndOfLine());

                    return base.GetUpdatedSyntaxRootAsync(document, updatedRoot, syntax, annotationOfSyntax, issue, cancellationToken);
                }
            }

            return base.GetUpdatedSyntaxRootAsync(document, root, syntax, annotationOfSyntax, issue, cancellationToken);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case BlockSyntax block: return GetUpdatedSyntax(block, issue);
                case EnumDeclarationSyntax e: return GetUpdatedSyntax(e, issue);
                case TypeDeclarationSyntax type: return GetUpdatedSyntax(type, issue);
                case NamespaceDeclarationSyntax ns: return GetUpdatedSyntax(ns, issue);
                default:
                    return syntax;
            }
        }

        private static BlockSyntax GetUpdatedSyntax(BlockSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            return GetUpdatedBlock(node, spaces);
        }

        private static EnumDeclarationSyntax GetUpdatedSyntax(EnumDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;
            var previousToken = closeBrace.GetPreviousToken();

            if (closeBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static TypeDeclarationSyntax GetUpdatedSyntax(TypeDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;
            var previousToken = closeBrace.GetPreviousToken();

            if (closeBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static NamespaceDeclarationSyntax GetUpdatedSyntax(NamespaceDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;
            var previousToken = closeBrace.GetPreviousToken();

            if (closeBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }
    }
}