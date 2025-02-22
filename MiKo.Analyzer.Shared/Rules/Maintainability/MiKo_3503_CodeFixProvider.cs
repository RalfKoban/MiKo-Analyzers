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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

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

                                                                  if (rewritten is TryStatementSyntax statement)
                                                                  {
                                                                      // move the statements inside the try block
                                                                      var block = statement.Block;
                                                                      var statements = block.Statements
                                                                                            .Insert(0, localDeclarationStatement.WithAdditionalLeadingSpaces(Constants.Indentation))
                                                                                            .Add(returnStatement.WithAdditionalLeadingSpaces(Constants.Indentation));

                                                                      return statement.WithBlock(block.WithStatements(statements));
                                                                  }

                                                                  if (rewritten is CatchClauseSyntax catchClause)
                                                                  {
                                                                      // move the statements inside the catch block
                                                                      var block = catchClause.Block;
                                                                      var statements = block.Statements
                                                                                            .Insert(0, localDeclarationStatement.WithAdditionalLeadingSpaces(Constants.Indentation))
                                                                                            .Add(returnStatement.WithAdditionalLeadingSpaces(Constants.Indentation));
                                                                      return catchClause.WithBlock(block.WithStatements(statements));
                                                                  }

                                                                  return rewritten;
                                                              });
            }

            return null;
        }
    }
}