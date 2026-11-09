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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BlockSyntax>().First();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BlockSyntax node)
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

            return syntax;
        }
    }
}