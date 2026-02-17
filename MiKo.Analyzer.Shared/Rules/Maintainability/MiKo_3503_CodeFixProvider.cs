using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3503_CodeFixProvider)), Shared]
    public sealed class MiKo_3503_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly SyntaxKind[] CatchKinds = { SyntaxKind.ThrowStatement, SyntaxKind.ReturnStatement };

        public override string FixableDiagnosticId => "MiKo_3503";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ReturnStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is ReturnStatementSyntax returnStatement
             && returnStatement.PreviousSibling() is TryStatementSyntax tryStatement
             && tryStatement.PreviousSibling() is LocalDeclarationStatementSyntax localDeclarationStatement)
            {
                var nodesToAdjust = new List<SyntaxNode>
                                        {
                                            localDeclarationStatement,
                                            tryStatement,
                                            returnStatement,
                                        };

                foreach (var catchClause in tryStatement.Catches)
                {
                    if (catchClause.Block.Statements.None(_ => _.IsAnyKind(CatchKinds)))
                    {
                        // seems we have a catch block that we need to adjust
                        nodesToAdjust.Add(catchClause);
                    }
                }

                return root.ReplaceNodes(
                                     nodesToAdjust,
                                     (original, rewritten) =>
                                                             {
                                                                 if (rewritten == returnStatement)
                                                                 {
                                                                     return null; // remove the return statement
                                                                 }

                                                                 if (rewritten == localDeclarationStatement)
                                                                 {
                                                                     return null; // remove the variable declaration statement
                                                                 }

                                                                 switch (rewritten)
                                                                 {
                                                                     case TryStatementSyntax statement:
                                                                         // move the statements inside the try block
                                                                         return statement.WithBlock(UpdateBlock(statement.Block, localDeclarationStatement, returnStatement));

                                                                     case CatchClauseSyntax catchClause:
                                                                         // move the statements inside the catch block
                                                                         return catchClause.WithBlock(UpdateBlock(catchClause.Block, localDeclarationStatement, returnStatement));

                                                                     default:
                                                                         return rewritten;
                                                                 }
                                                             });
            }

            return null;
        }

        private static BlockSyntax UpdateBlock(BlockSyntax block, LocalDeclarationStatementSyntax localDeclarationStatement, ReturnStatementSyntax returnStatement)
        {
            var localDeclaration = localDeclarationStatement;
            var declarationSyntax = localDeclaration.Declaration;
            var declarationVariable = declarationSyntax.Variables[0];

            var blockStatements = block.Statements.ToList();

            if (declarationVariable.Initializer is null)
            {
                var name = declarationVariable.GetName();

                // only add a type initializer if the block does not contain the variable's assignment
                if (blockStatements.None(_ => _.IsAssignmentOf(name)))
                {
                    var updatedDeclarationVariable = declarationVariable.WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(declarationSyntax.Type)));

                    localDeclaration = localDeclarationStatement.WithDeclaration(declarationSyntax.WithVariables(declarationSyntax.Variables.Replace(declarationVariable, updatedDeclarationVariable)));
                }
            }

            blockStatements.Insert(0, localDeclaration.WithAdditionalLeadingSpaces(Constants.Indentation));
            blockStatements.Add(returnStatement.WithAdditionalLeadingSpaces(Constants.Indentation));

            return block.WithStatements(blockStatements.ToSyntaxList());
        }
    }
}