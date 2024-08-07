using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3202_CodeFixProvider)), Shared]
    public sealed class MiKo_3202_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly SyntaxKind[] ReturnPoints = { SyntaxKind.ArrowExpressionClause, SyntaxKind.ReturnStatement };

        public override string FixableDiagnosticId => "MiKo_3202";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case IfStatementSyntax _:
                    case ConditionalExpressionSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case IfStatementSyntax ifStatement:
                    return GetUpdatedSyntaxRootForIfStatement(document, root, ifStatement);

                case ConditionalExpressionSyntax conditional:
                    return GetUpdatedSyntaxRootForConditional(document, root, conditional);

                default:
                    return root;
            }
        }

        private static SyntaxNode GetUpdatedSyntaxRootForIfStatement(Document document, SyntaxNode root, IfStatementSyntax syntax)
        {
            var newIf = syntax.WithCondition(GetUpdatedCondition(document, syntax.Condition));

            var elsePart = syntax.Else?.Statement;

            if (elsePart != null)
            {
                // seems we have an else part, so switch
                return root.ReplaceNode(syntax, newIf.WithStatement(elsePart).WithElse(SyntaxFactory.ElseClause(syntax.Statement)));
            }

            // seems we have no else, so it must be the next statement inside the block
            if (syntax.Parent is BlockSyntax block)
            {
                var statements = block.Statements.ToArray();

                var index = statements.IndexOf(syntax);
                var other = statements.Skip(index + 1).FirstOrDefault();

                if (other != null)
                {
                    // find the return statement inside the if
                    var statement = syntax.Statement is BlockSyntax b
                                    ? b.Statements.First()
                                    : syntax.Statement;

                    // we found a follow up, so we fix the block first and then the document
                    var newBlock = block.ReplaceNodes(new[] { syntax, other }, (_, rewritten) => rewritten.IsKind(SyntaxKind.IfStatement)
                                                                                                 ? newIf.WithStatement(SyntaxFactory.Block(other.WithoutLeadingEndOfLine()))
                                                                                                 : statement.WithAdditionalLeadingSpaces(Constants.Indentation * -1).WithLeadingEmptyLine());

                    return root.ReplaceNode(block, newBlock);
                }
            }

            return null;
        }

        private static SyntaxNode GetUpdatedSyntaxRootForConditional(Document document, SyntaxNode root, ConditionalExpressionSyntax syntax)
        {
            var newCondition = GetUpdatedCondition(document, syntax.Condition);

            var returnPoint = syntax.FirstAncestor<SyntaxNode>(ReturnPoints);

            var oldWhenTrue = syntax.WhenTrue;
            var oldWhenFalse = syntax.WhenFalse;

            var removeCommentFromStatement = false;
            var applyCommentFromTrue = false;

            // seems we have different lines, so there might be comments to adjust as well
            if (oldWhenTrue.GetStartingLine() != oldWhenFalse.GetStartingLine())
            {
                // different line, so apply comment from complete statement
                applyCommentFromTrue = oldWhenTrue.HasTrailingComment();
                removeCommentFromStatement = returnPoint.HasTrailingComment();
            }

            var newWhenTrue = oldWhenTrue.HasTrailingEndOfLine()
                              ? oldWhenFalse.WithTrailingNewLine()
                              : oldWhenFalse.WithTrailingSpace();

            if (removeCommentFromStatement)
            {
                newWhenTrue = newWhenTrue.WithTrailingTriviaFrom(returnPoint);
            }

            var newWhenFalse = oldWhenTrue.WithoutTrailingTrivia();

            var newConditional = syntax.WithCondition(newCondition)
                                       .WithWhenTrue(newWhenTrue)
                                       .WithWhenFalse(newWhenFalse);

            var nodeToReplace = syntax.Parent is ParenthesizedExpressionSyntax parenthesized
                                ? (SyntaxNode)parenthesized
                                : syntax;

            var newReturnPoint = returnPoint.ReplaceNode(nodeToReplace, newConditional);

            if (removeCommentFromStatement)
            {
                newReturnPoint = newReturnPoint.WithTrailingNewLine();
            }

            if (applyCommentFromTrue)
            {
                newReturnPoint = newReturnPoint.WithTrailingTriviaFrom(oldWhenTrue);
            }

            return root.ReplaceNode(returnPoint, newReturnPoint);
        }

        private static ExpressionSyntax GetUpdatedCondition(Document document, ExpressionSyntax condition)
        {
            var newCondition = condition is ParenthesizedExpressionSyntax parenthesized
                               ? parenthesized.Expression
                               : condition;

            return InvertCondition(document, newCondition).WithTriviaFrom(condition);
        }
    }
}