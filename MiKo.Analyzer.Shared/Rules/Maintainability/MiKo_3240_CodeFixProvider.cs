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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3240_CodeFixProvider)), Shared]
    public sealed class MiKo_3240_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3240";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BlockSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is BlockSyntax block)
            {
                var statements = block.Statements;

                if (statements.Count > 1 && statements[0] is LocalDeclarationStatementSyntax localDeclarationStatement && statements[1] is IfStatementSyntax ifStatement)
                {
                    var value = localDeclarationStatement.Declaration.Variables[0].Initializer?.Value;

                    // adjust the first return statement in if block
                    var updatedIfStatement = ifStatement.WithLeadingTriviaFrom(localDeclarationStatement);
                    var returnStatement = updatedIfStatement.FirstDescendant<ReturnStatementSyntax>();
                    updatedIfStatement = updatedIfStatement.ReplaceNode(returnStatement, returnStatement.WithExpression(value));

                    var updatedStatements = statements;

                    var elseClause = ifStatement.Else;

                    if (elseClause?.Statement is BlockSyntax elseBlock)
                    {
                        // move declaration statement into else clause
                        var elseBlockStatements = elseBlock.Statements;
                        var updatedElseBlockStatements = elseBlockStatements.Insert(0, localDeclarationStatement.WithLeadingTriviaFrom(elseBlockStatements[0]));
                        var updatedElseBlock = elseBlock.WithStatements(updatedElseBlockStatements);

                        updatedStatements = statements.Replace(ifStatement, updatedIfStatement.WithElse(elseClause.ReplaceNode(elseBlock, updatedElseBlock)));
                    }
                    else
                    {
                        // move declaration statement after if statement
                        updatedStatements = statements.Replace(ifStatement, updatedIfStatement)
                                                      .Insert(2, localDeclarationStatement.WithLeadingTriviaFrom(ifStatement));
                    }

                    return block.WithStatements(updatedStatements.RemoveAt(0)); // remove the declaration
                }
            }

            return syntax;
        }
    }
}