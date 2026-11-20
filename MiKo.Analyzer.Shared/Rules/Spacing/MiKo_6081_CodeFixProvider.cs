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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6081_CodeFixProvider)), Shared]
    public sealed class MiKo_6081_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6081";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            SyntaxToken openBrace = FindOpenBrace(syntax);

            if (openBrace.IsDefaultValue() is false)
            {
                var previousToken = openBrace.GetPreviousToken();

                if (openBrace.IsOnSameLineAsEndOf(previousToken))
                {
                    var updatedRoot = root.ReplaceToken(previousToken, previousToken.WithForcedEndOfLine());

                    return base.GetUpdatedSyntaxRootAsync(document, updatedRoot, syntax, annotationOfSyntax, issue, cancellationToken);
                }
            }

            return base.GetUpdatedSyntaxRootAsync(document, root, syntax, annotationOfSyntax, issue, cancellationToken);

            SyntaxToken FindOpenBrace(SyntaxNode syntaxNode)
            {
                switch (syntaxNode)
                {
                    case BlockSyntax node: return node.OpenBraceToken;
                    case AccessorListSyntax accessor: return accessor.OpenBraceToken;
                    default:
                        return default;
                }
            }
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case AccessorListSyntax a: return GetUpdatedSyntax(a, issue);
                case BlockSyntax block: return GetUpdatedSyntax(block, issue);
                case EnumDeclarationSyntax e: return GetUpdatedSyntax(e, issue);
                case NamespaceDeclarationSyntax ns: return GetUpdatedSyntax(ns, issue);
                case SwitchStatementSyntax s: return GetUpdatedSyntax(s, issue);
                case TypeDeclarationSyntax type: return GetUpdatedSyntax(type, issue);
                default:
                    return syntax;
            }
        }

        private static AccessorListSyntax GetUpdatedSyntax(AccessorListSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            return node.WithOpenBraceToken(node.OpenBraceToken.WithAdditionalLeadingSpaces(spaces));
        }

        private static BlockSyntax GetUpdatedSyntax(BlockSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            return GetUpdatedBlock(node, spaces);
        }

        private static SwitchStatementSyntax GetUpdatedSyntax(SwitchStatementSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var openBrace = node.OpenBraceToken;
            var previousToken = openBrace.GetPreviousToken();

            if (openBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithOpenBraceToken(node.OpenBraceToken.WithAdditionalLeadingSpaces(spaces));
        }

        private static EnumDeclarationSyntax GetUpdatedSyntax(EnumDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var openBrace = node.OpenBraceToken;
            var previousToken = openBrace.GetPreviousToken();

            if (openBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static TypeDeclarationSyntax GetUpdatedSyntax(TypeDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var openBrace = node.OpenBraceToken;
            var previousToken = openBrace.GetPreviousToken();

            if (openBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static NamespaceDeclarationSyntax GetUpdatedSyntax(NamespaceDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var openBrace = node.OpenBraceToken;
            var previousToken = openBrace.GetPreviousToken();

            if (openBrace.IsOnSameLineAsEndOf(previousToken))
            {
                return node.ReplaceToken(previousToken, previousToken.WithAdditionalEndOfLine())
                           .WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithOpenBraceToken(openBrace.WithAdditionalLeadingSpaces(spaces));
        }
    }
}