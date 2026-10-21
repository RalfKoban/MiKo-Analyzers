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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3242_CodeFixProvider)), Shared]
    public sealed class MiKo_3242_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3242";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is IfStatementSyntax statement && statement.Condition is BinaryExpressionSyntax b && b.Left is IdentifierNameSyntax identifier)
            {
                var invocation = statement.Statement.FirstDescendant<InvocationExpressionSyntax>();

                if (invocation.Parent is ExpressionStatementSyntax invocationStatement)
                {
                    var updatedInvocation = invocation.WithoutTrivia()
                                                      .WithExpression(MemberBinding(nameof(EventHandler.Invoke)));

                    var updatedSyntax = SyntaxFactory.ConditionalAccessExpression(identifier, updatedInvocation);

                    var updatedStatement = Statement(updatedSyntax).WithTriviaFrom(statement);

                    // re-add comments (either from statement or from invocation)
                    if (invocationStatement.HasLeadingComment())
                    {
                        updatedStatement = updatedStatement.WithLeadingTriviaFrom(invocationStatement)
                                                           .WithAdditionalLeadingSpaces(-Constants.Indentation); // out-dent the statement
                    }

                    var semicolonToken = invocationStatement.SemicolonToken;

                    if (semicolonToken.HasTrailingComment())
                    {
                        updatedStatement = updatedStatement.WithSemicolonToken(updatedStatement.SemicolonToken.WithTrailingTriviaFrom(semicolonToken));
                    }

                    return updatedStatement;
                }
            }

            return syntax;
        }
    }
}