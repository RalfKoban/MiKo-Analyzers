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

        private static SyntaxNode GetUpdatedSyntax(BlockSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;

            if (node.Statements.LastOrDefault() is StatementSyntax statement && closeBrace.IsOnSameLineAsEndOf(statement))
            {
                return node.ReplaceNode(statement, statement.WithEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static SyntaxNode GetUpdatedSyntax(EnumDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;

            if (node.Members.LastOrDefault() is MemberDeclarationSyntax member && closeBrace.IsOnSameLineAsEndOf(member))
            {
                return node.ReplaceNode(member, member.WithEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static SyntaxNode GetUpdatedSyntax(TypeDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;

            if (node.Members.LastOrDefault() is MemberDeclarationSyntax member && closeBrace.IsOnSameLineAsEndOf(member))
            {
                return node.ReplaceNode(member, member.WithEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }

        private static SyntaxNode GetUpdatedSyntax(NamespaceDeclarationSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedAdditionalSpaces(issue);

            var closeBrace = node.CloseBraceToken;

            if (node.Members.LastOrDefault() is MemberDeclarationSyntax member && closeBrace.IsOnSameLineAsEndOf(member))
            {
                return node.ReplaceNode(member, member.WithEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithAdditionalLeadingSpaces(spaces));
        }
    }
}