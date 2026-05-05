using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3505_CodeFixProvider)), Shared]
    public sealed class MiKo_3505_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3505";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ReturnStatementSyntax>().FirstOrDefault();

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
            if (syntax is ReturnStatementSyntax returnStatement && returnStatement.PreviousSibling() is TryStatementSyntax tryStatement)
            {
                var nodesToAdjust = new List<SyntaxNode>
                                        {
                                            tryStatement,
                                            returnStatement,
                                        };

                var updatedRoot = root.ReplaceNodes(
                                                nodesToAdjust,
                                                (original, rewritten) =>
                                                                        {
                                                                            if (rewritten == returnStatement)
                                                                            {
                                                                                return null; // remove the return statement
                                                                            }

                                                                            if (rewritten is TryStatementSyntax statement)
                                                                            {
                                                                                // move the return statement inside the try block
                                                                                return statement.WithBlock(UpdateBlock(statement.Block, returnStatement));
                                                                            }

                                                                            return rewritten;
                                                                        });

                return updatedRoot;
            }

            return null;
        }

        private static BlockSyntax UpdateBlock(BlockSyntax block, ReturnStatementSyntax returnStatement)
        {
            var blockStatements = block.Statements.Add(returnStatement.WithAdditionalLeadingSpaces(Constants.Indentation));

            return block.WithStatements(blockStatements);
        }
    }
}