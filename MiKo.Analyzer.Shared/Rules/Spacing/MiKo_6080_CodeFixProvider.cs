using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6080_CodeFixProvider)), Shared]
    public sealed class MiKo_6080_CodeFixProvider : IndentedSpacingCodeFixProvider<BlockSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6080";

        protected override BlockSyntax GetUpdatedSyntax(BlockSyntax node, in int spaces)
        {
            var closeBrace = node.CloseBraceToken;

            if (node.Statements.LastOrDefault() is StatementSyntax statement && closeBrace.IsOnSameLineAsEndOf(statement))
            {
                return node.ReplaceNode(statement, statement.WithEndOfLine())
                           .WithCloseBraceToken(closeBrace.WithLeadingSpaces(spaces));
            }

            return node.WithCloseBraceToken(closeBrace.WithLeadingSpaces(spaces));
        }
    }
}