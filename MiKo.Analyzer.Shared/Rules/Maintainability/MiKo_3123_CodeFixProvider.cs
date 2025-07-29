using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3123_CodeFixProvider)), Shared]
    public sealed class MiKo_3123_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3123";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var node = syntaxNodes.OfType<CatchClauseSyntax>().FirstOrDefault();

            return node?.Parent;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is TryStatementSyntax statement)
            {
                if (root is CompilationUnitSyntax compilationUnit && compilationUnit.Usings.Any(_ => _.GetName() == Constants.Names.DefaultNUnitNamespace))
                {
                    return GetUpdatedSyntaxRootForNUnit(root, statement);
                }

                return GetUpdatedSyntaxRoot(root, statement);
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, TryStatementSyntax statement)
        {
            if (statement.Finally is null)
            {
                var spaces = statement.GetPositionWithinStartLine();

                return root.ReplaceNode(statement, statement.Block.Statements.Select(_ => _.WithLeadingSpaces(spaces)));
            }

            return root.ReplaceNode(statement, statement.Without(statement.Catches));
        }

        private static SyntaxNode GetUpdatedSyntaxRootForNUnit(SyntaxNode root, TryStatementSyntax statement)
        {
            var spaces = statement.GetPositionWithinStartLine();

            if (statement.Finally is null)
            {
                var assertionStatement = CreateAssertionStatement(statement, spaces + Constants.Indentation);

                return root.ReplaceNode(statement, assertionStatement.WithTriviaFrom(statement));
            }
            else
            {
                var assertionStatement = CreateAssertionStatement(statement, spaces + Constants.Indentation + Constants.Indentation);

                return root.ReplaceNode(statement, statement.Without(statement.Catches).WithBlock(SyntaxFactory.Block(assertionStatement)));
            }
        }

        private static ExpressionStatementSyntax CreateAssertionStatement(TryStatementSyntax statement, int spaces)
        {
            var updatedBlock = SyntaxFactory.Block(
                                               SyntaxKind.OpenBraceToken.AsToken().WithEndOfLine(),
                                               statement.Block.Statements.Select(_ => _.WithoutLeadingTrivia().WithLeadingSpaces(spaces)).ToSyntaxList(),
                                               SyntaxKind.CloseBraceToken.AsToken());

            var assertion = AssertThat(Argument(SyntaxFactory.ParenthesizedLambdaExpression(updatedBlock)), Throws("Nothing").WithLeadingSpace());

            return SyntaxFactory.ExpressionStatement(assertion);
        }
    }
}