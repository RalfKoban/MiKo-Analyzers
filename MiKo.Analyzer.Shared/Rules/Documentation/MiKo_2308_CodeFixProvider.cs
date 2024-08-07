using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2308_CodeFixProvider)), Shared]
    public sealed class MiKo_2308_CodeFixProvider : CommentCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2308";

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

                case LocalDeclarationStatementSyntax declaration when HasIssue(declaration):
                {
                    // remove trivia from declaration and add new item
                    var newDeclaration = declaration.RemoveTrivia(trivia)
                                                    .WithAdditionalLeadingTrivia(trivia, SyntaxFactory.ElasticCarriageReturnLineFeed);

                    return root.ReplaceNode(declaration, newDeclaration);
                }
            }

            return root.RemoveTrivia(trivia);
        }

        private static bool HasIssue(LocalDeclarationStatementSyntax declaration)
        {
            foreach (var node in declaration.DescendantNodes())
            {
                switch (node)
                {
                    case InitializerExpressionSyntax _:
#if VS2022
                    case CollectionExpressionSyntax _:
#endif
                        return true;
                }
            }

            return false;
        }
    }
}