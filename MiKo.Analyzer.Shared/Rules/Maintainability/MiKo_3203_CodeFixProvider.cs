using System;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3203_CodeFixProvider)), Shared]
    public sealed class MiKo_3203_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3203";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected override async Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            if (syntax is IfStatementSyntax ifStatement && syntax.Parent is BlockSyntax block)
            {
                var statements = block.Statements.ToArray();

                var index = statements.IndexOf(ifStatement);

                if (index < statements.Length)
                {
                    var others = statements.Skip(index + 1).ToList();
                    others[0] = others[0].WithoutLeadingEndOfLine();

                    var spaces = others[0].GetPositionWithinStartLine();

                    var condition = ifStatement.Condition;

                    var invertedCondition = await InvertConditionAsync(condition, document, cancellationToken).ConfigureAwait(false);

                    var newIf = ifStatement.WithCondition(invertedCondition.WithTriviaFrom(condition))
                                           .WithCloseParenToken(ifStatement.CloseParenToken.WithoutTrailingTrivia())
                                           .WithStatement(GetUpdatedBlock(SyntaxFactory.Block(others), spaces)); // adjust spacing

                    var comment = GetComment(ifStatement);

                    if (comment.Length > 0)
                    {
                        newIf = newIf.WithAdditionalLeadingTrivia(comment)
                                     .WithAdditionalLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                                     .WithAdditionalLeadingTriviaFrom(ifStatement);
                    }

                    return root.ReplaceNodes(statements.Skip(index), (original, rewritten) => original == ifStatement ? newIf : null);
                }
            }

            return root;
        }

        private static SyntaxTrivia[] GetComment(IfStatementSyntax ifStatement)
        {
            var statement = ifStatement.Statement;

            var closeParenToken = ifStatement.CloseParenToken;

            if (closeParenToken.HasTrailingComment())
            {
                return closeParenToken.GetComment();
            }

            if (statement is BlockSyntax block)
            {
                statement = block.Statements.FirstOrDefault();
            }

            if (statement is ContinueStatementSyntax cs)
            {
                var continueKeyword = cs.ContinueKeyword;

                if (continueKeyword.HasComment())
                {
                    return continueKeyword.GetComment();
                }

                var semicolonToken = cs.SemicolonToken;

                if (semicolonToken.HasComment())
                {
                    return semicolonToken.GetComment();
                }
            }

            return Array.Empty<SyntaxTrivia>();
        }
    }
}