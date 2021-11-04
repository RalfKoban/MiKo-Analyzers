using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2308_CodeFixProvider)), Shared]
    public sealed class MiKo_2308_CodeFixProvider : SingleLineCommentCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2308_CommentPlacedAfterCodeAnalyzer.Id;

        protected override string Title => Resources.MiKo_2308_CodeFixTitle;

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten) => original;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxTrivia trivia, Diagnostic diagnostic)
        {
            if (trivia.Token.Parent is BlockSyntax block)
            {
                // remove trivia from block and add new item
                var newBlock = block.ReplaceTrivia(trivia, SyntaxFactory.ElasticMarker);

                var statement = newBlock.Statements.LastOrDefault();
                if (statement != null)
                {
                    newBlock = newBlock.ReplaceNode(statement, statement.WithAdditionalLeadingTrivia(trivia, SyntaxFactory.ElasticCarriageReturnLineFeed));

                    return root.ReplaceNode(block, newBlock);
                }
            }

            // TODO RKN: Find trivia in new root
            return root.ReplaceTrivia(trivia, SyntaxFactory.ElasticMarker);
        }
    }
}