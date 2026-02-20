using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3124_CodeFixProvider)), Shared]
    public sealed class MiKo_3124_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3124";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root, syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            if (syntax is ExpressionStatementSyntax statement && statement.FirstAncestor<TryStatementSyntax>() is TryStatementSyntax tryStatement && tryStatement.Parent is BlockSyntax outerBlock)
            {
                var nodeToRemove = statement.Parent is BlockSyntax block && block.Parent.IsKind(SyntaxKind.FinallyClause) is false && block.Statements.Count is 1
                                   ? (SyntaxNode)block
                                   : statement;

                var nodeToAdjust = nodeToRemove.NextSibling();

                BlockSyntax updatedBlock;

                if (nodeToAdjust != null)
                {
                    // adjust the leading trivia of the node to adjust
                    updatedBlock = outerBlock.ReplaceNodes(new[] { nodeToRemove, nodeToAdjust }, (original, rewritten) => original == nodeToAdjust ? rewritten.WithoutLeadingEndOfLine() : null);
                }
                else
                {
                    updatedBlock = outerBlock.Without(nodeToRemove);
                }

                return root.ReplaceNode(outerBlock, updatedBlock.AddStatements(statement.WithLeadingSpaces(tryStatement.GetPositionWithinStartLine()).WithLeadingEmptyLine()));
            }

            return root;
        }
    }
}