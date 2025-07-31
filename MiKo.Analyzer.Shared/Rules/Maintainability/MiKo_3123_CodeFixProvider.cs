using System;
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

        private static SyntaxNode GetUpdatedSyntaxRootForNUnit(SyntaxNode root, TryStatementSyntax tryStatement)
        {
            var spaces = tryStatement.GetPositionWithinStartLine();

            if (tryStatement.Finally is null)
            {
                var assertionStatement = CreateNUnitAssertionStatement(tryStatement, spaces + Constants.Indentation);

                return root.ReplaceNode(tryStatement, assertionStatement.WithTriviaFrom(tryStatement));
            }
            else
            {
                var assertionStatement = CreateNUnitAssertionStatement(tryStatement, spaces + Constants.Indentation + Constants.Indentation);

                return root.ReplaceNode(tryStatement, tryStatement.Without(tryStatement.Catches).WithBlock(SyntaxFactory.Block(assertionStatement)));
            }
        }

        private static SyntaxNode GetUpdatedSyntaxRootForXunit(SyntaxNode root, TryStatementSyntax tryStatement)
        {
            if (tryStatement.Finally is null)
            {
                var spaces = tryStatement.GetPositionWithinStartLine();

                return root.ReplaceNode(tryStatement, CreateStatementsForXunit(tryStatement).Select(_ => _.WithLeadingSpaces(spaces)));
            }

            return root.ReplaceNode(tryStatement, tryStatement.Without(tryStatement.Catches));
        }

        private static IEnumerable<StatementSyntax> CreateStatementsForXunit(TryStatementSyntax tryStatement)
        {
            var statements = tryStatement.Block.Statements;
            var assertFailStatements = statements.Where(IsAssertFail).ToList();
            var statementsWithoutAssertFails = statements.Except(assertFailStatements);

            var catches = tryStatement.Catches;

            if (catches.Count > 0)
            {
                var additionalStatements = catches.SelectMany(_ => _.Block.Statements).ToList();

                if (additionalStatements.Any(IsAssertFail))
                {
                    return statementsWithoutAssertFails;
                }

                if (additionalStatements.Any(IsAssert))
                {
                    var assertionStatement = CreateXunitAssertionStatement(tryStatement, catches[0], statementsWithoutAssertFails);

                    additionalStatements.Insert(0, assertionStatement);

                    return additionalStatements;
                }

                if (assertFailStatements.Count > 0)
                {
                    var assertionStatement = CreateXunitAssertionStatement(tryStatement, catches[0], statementsWithoutAssertFails);

                    return new[] { assertionStatement };
                }

                return statementsWithoutAssertFails.Concat(additionalStatements);
            }

            return statementsWithoutAssertFails;
        }

        private static ExpressionStatementSyntax CreateNUnitAssertionStatement(TryStatementSyntax tryStatement, in int spaces)
        {
            var statements = tryStatement.Block.Statements;
            var assertFailStatements = statements.OfType<ExpressionStatementSyntax>().Where(IsAssertFail).ToList();

            var arguments = new List<ArgumentSyntax>
                                {
                                    Argument(ParenthesizedLambda(Block(statements.Except(assertFailStatements), spaces))),
                                    ThrowsArgument(tryStatement.Catches, assertFailStatements).WithLeadingSpace(),
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

        private static StatementSyntax CreateXunitAssertionStatement(TryStatementSyntax tryStatement, CatchClauseSyntax catchClause, IEnumerable<StatementSyntax> statements)
        {
            var spaces = tryStatement.GetPositionWithinStartLine();
            var lambda = Argument(ParenthesizedLambda(Block(statements, spaces + Constants.Indentation)));

            var catchClauseDeclaration = catchClause.Declaration;
            var exceptionType = catchClauseDeclaration?.Type ?? SyntaxFactory.ParseTypeName(nameof(Exception));
            var exceptionIdentifier = catchClauseDeclaration.GetName();

            var invocation = Invocation("Assert", "Throws", exceptionType, lambda);

            if (exceptionIdentifier.IsNullOrEmpty())
            {
                return SyntaxFactory.ExpressionStatement(invocation).WithTriviaFrom(tryStatement);
            }

            var declarator = SyntaxFactory.VariableDeclarator(exceptionIdentifier).WithInitializer(SyntaxFactory.EqualsValueClause(invocation));
            var declaration = SyntaxFactory.VariableDeclaration(exceptionType, declarator.ToSeparatedSyntaxList());
            var localDeclaration = SyntaxFactory.LocalDeclarationStatement(declaration);

            return localDeclaration.WithLeadingTriviaFrom(tryStatement).WithTrailingEmptyLine();
        }

        private static BlockSyntax Block(IEnumerable<StatementSyntax> statements, int spaces)
        {
            return SyntaxFactory.Block(
                                   SyntaxKind.OpenBraceToken.AsToken().WithEndOfLine(),
                                   statements.Select(_ => _.WithoutLeadingTrivia().WithLeadingSpaces(spaces)).ToSyntaxList(),
                                   SyntaxKind.CloseBraceToken.AsToken());
        }

        private static bool IsAssert(StatementSyntax statement) => statement is ExpressionStatementSyntax node && IsAssert(node);

        private static bool IsAssert(ExpressionStatementSyntax statement) => statement.Expression is InvocationExpressionSyntax i && i.GetIdentifierName() is "Assert" && i.Expression.GetName() != "Fail";

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