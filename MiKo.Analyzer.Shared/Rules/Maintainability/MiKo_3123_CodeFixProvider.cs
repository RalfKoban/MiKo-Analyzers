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
            if (syntax is TryStatementSyntax statement && root is CompilationUnitSyntax compilationUnit)
            {
                var usingDirectives = compilationUnit.Usings;

                if (usingDirectives.Any(_ => _.GetName() is Constants.Names.DefaultNUnitNamespace))
                {
                    return GetUpdatedSyntaxRootForNUnit(root, statement);
                }

                if (usingDirectives.Any(_ => _.GetName() is Constants.Names.DefaultXUnitNamespace))
                {
                    return GetUpdatedSyntaxRootForXunit(root, statement);
                }
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxRootForXunit(SyntaxNode root, TryStatementSyntax statement)
        {
            if (statement.Finally is null)
            {
                var spaces = statement.GetPositionWithinStartLine();

                return root.ReplaceNode(statement, statement.Block.Statements.Where(_ => IsAssertFail(_) is false).Select(_ => _.WithLeadingSpaces(spaces)));
            }

            return root.ReplaceNode(statement, statement.Without(statement.Catches));
        }

        private static SyntaxNode GetUpdatedSyntaxRootForNUnit(SyntaxNode root, TryStatementSyntax statement)
        {
            var spaces = statement.GetPositionWithinStartLine();

            if (statement.Finally is null)
            {
                var assertionStatement = CreateNUnitAssertionStatement(statement, spaces + Constants.Indentation);

                return root.ReplaceNode(statement, assertionStatement.WithTriviaFrom(statement));
            }
            else
            {
                var assertionStatement = CreateNUnitAssertionStatement(statement, spaces + Constants.Indentation + Constants.Indentation);

                return root.ReplaceNode(statement, statement.Without(statement.Catches).WithBlock(SyntaxFactory.Block(assertionStatement)));
            }
        }

        private static ExpressionStatementSyntax CreateNUnitAssertionStatement(TryStatementSyntax statement, int spaces)
        {
            var statements = statement.Block.Statements;
            var assertFailStatements = statements.OfType<ExpressionStatementSyntax>().Where(IsAssertFail).ToList();

            var updatedBlock = SyntaxFactory.Block(
                                               SyntaxKind.OpenBraceToken.AsToken().WithEndOfLine(),
                                               statements.Except(assertFailStatements).Select(_ => _.WithoutLeadingTrivia().WithLeadingSpaces(spaces)).ToSyntaxList(),
                                               SyntaxKind.CloseBraceToken.AsToken());

            var arguments = new List<ArgumentSyntax>
                                {
                                    Argument(SyntaxFactory.ParenthesizedLambdaExpression(updatedBlock)),
                                    ThrowsArgument(statement.Catches, assertFailStatements).WithLeadingSpace(),
                                };

            if (assertFailStatements.Count > 0)
            {
                if (assertFailStatements[0].Expression is InvocationExpressionSyntax i && i.ArgumentList?.Arguments is SeparatedSyntaxList<ArgumentSyntax> args && args.Count > 0)
                {
                    arguments.Add(args[0].WithLeadingSpace());
                }
            }
            else
            {
                // TODO RKN: Append more?
            }

            return SyntaxFactory.ExpressionStatement(AssertThat(arguments.ToArray()));
        }

        private static bool IsAssertFail(StatementSyntax statement) => statement is ExpressionStatementSyntax node && IsAssertFail(node);

        private static bool IsAssertFail(ExpressionStatementSyntax statement) => statement.Expression is InvocationExpressionSyntax i && i.GetIdentifierName() is "Assert" && i.Expression.GetName() is "Fail";

        private static ArgumentSyntax ThrowsArgument(in SyntaxList<CatchClauseSyntax> catches, List<ExpressionStatementSyntax> assertFailStatements)
        {
            var needsException = assertFailStatements.Count > 0;

            if (catches.Count > 0)
            {
                var catchClause = catches[0];

                if (catchClause.Declaration is CatchDeclarationSyntax catchDeclaration)
                {
                    if (catchClause.Block.Statements.Count is 0 && needsException is false)
                    {
                        return Throws("Nothing");
                    }

                    // TODO RKN: check for assertions in catch block, and use those
                    return Throws(catchDeclaration.Type);
                }
            }

            return Throws(needsException ? "Exception" : "Nothing");
        }
    }
}