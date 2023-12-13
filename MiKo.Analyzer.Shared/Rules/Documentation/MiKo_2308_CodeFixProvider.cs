using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2308_CodeFixProvider)), Shared]
    public sealed class MiKo_2308_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2308_CommentPlacedAfterCodeAnalyzer.Id;

        protected override string Title => Resources.MiKo_2308_CodeFixTitle;

        protected override SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten) => original;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxTrivia trivia, Diagnostic issue)
        {
            switch (trivia.Token.Parent)
            {
                case BlockSyntax block:
                {
                    // remove trivia from block and add new item
                    var newBlock = block.RemoveTrivia(trivia);

                    var statement = newBlock.Statements.LastOrDefault();

                    if (statement != null)
                    {
                        newBlock = newBlock.ReplaceNode(statement, statement.WithAdditionalLeadingTrivia(trivia, SyntaxFactory.ElasticCarriageReturnLineFeed));

                        return root.ReplaceNode(block, newBlock);
                    }

                    break;
                }

                case LocalDeclarationStatementSyntax declaration when declaration.DescendantNodes<InitializerExpressionSyntax>().Any():
                {
                    // remove trivia from declaration and add new item
                    var newDeclaration = declaration.RemoveTrivia(trivia)
                                                    .WithAdditionalLeadingTrivia(trivia, SyntaxFactory.ElasticCarriageReturnLineFeed);

                    return root.ReplaceNode(declaration, newDeclaration);
                }
            }

            return root.RemoveTrivia(trivia);
        }
    }
}