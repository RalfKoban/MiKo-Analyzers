using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

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

                for (int index = 0, count = usingDirectives.Count; index < count; index++)
                {
                    var usingName = usingDirectives[index].GetName();

                    switch (usingName)
                    {
                        case Constants.Names.DefaultNUnitNamespace: return GetUpdatedSyntaxRootForNUnit(root, statement);
                        case Constants.Names.DefaultXUnitNamespace: return GetUpdatedSyntaxRootForXunit(root, statement);
                    }
                }
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxRootForXunit(SyntaxNode root, TryStatementSyntax tryStatement)
        {
            var testMethod = tryStatement.FirstAncestor<MethodDeclarationSyntax>();
            var spaces = tryStatement.GetPositionWithinStartLine();

            IReadOnlyList<StatementSyntax> statementsForXunit;
            MethodDeclarationSyntax updatedTestMethod;

            if (tryStatement.Finally is null)
            {
                statementsForXunit = CreateStatementsForXunit(tryStatement, testMethod, spaces + Constants.Indentation);

                updatedTestMethod = testMethod.ReplaceNode(tryStatement, statementsForXunit.Select(_ => _.WithLeadingSpaces(spaces)));
            }
            else
            {
                // we have a finally block, so we need to ensure that the catch statements are added to the try block
                statementsForXunit = CreateStatementsForXunit(tryStatement, testMethod, spaces + Constants.Indentation + Constants.Indentation);

                var block = tryStatement.Block.WithStatements(statementsForXunit.Select(_ => _.WithLeadingSpaces(spaces + Constants.Indentation)).ToSyntaxList());

                updatedTestMethod = testMethod.ReplaceNode(tryStatement, tryStatement.Without(tryStatement.Catches).WithBlock(block));
            }

            if (statementsForXunit.SelectMany(_ => _.DescendantNodes()).OfType<AwaitExpressionSyntax>().Any())
            {
                // seems we added an await expression, so we need to ensure that the method is async
                if (testMethod.Modifiers.None(SyntaxKind.AsyncKeyword))
                {
                    return root.ReplaceNode(testMethod, updatedTestMethod.WithReturnType(nameof(Task).AsTypeSyntax()).WithAdditionalModifier(SyntaxKind.AsyncKeyword))
                               .WithUsing("System.Threading.Tasks");
                }
            }

            return root.ReplaceNode(testMethod, updatedTestMethod);
        }

        private static IReadOnlyList<StatementSyntax> CreateStatementsForXunit(TryStatementSyntax tryStatement, MethodDeclarationSyntax testMethod, int spaces)
        {
            var statements = tryStatement.Block.Statements;
            var assertFailStatements = statements.Where(IsAssertFail).ToList();
            var statementsWithoutAssertFails = statements.Except(assertFailStatements).ToList();

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
                    var assertionStatement = CreateXunitAssertionStatement(tryStatement, catches[0], statementsWithoutAssertFails, testMethod, spaces);

                    additionalStatements.Insert(0, assertionStatement);

                    // ensure that the first additional statement is separated by an empty line
                    additionalStatements[1] = additionalStatements[1].WithLeadingEmptyLine();

                    return additionalStatements;
                }

                if (assertFailStatements.Count > 0)
                {
                    var assertionStatement = CreateXunitAssertionStatement(tryStatement, catches[0], statementsWithoutAssertFails, testMethod, spaces);

                    return new[] { assertionStatement };
                }

                statementsWithoutAssertFails.AddRange(additionalStatements);
            }

            return statementsWithoutAssertFails;
        }

        private static StatementSyntax CreateXunitAssertionStatement(TryStatementSyntax tryStatement, CatchClauseSyntax catchClause, IEnumerable<StatementSyntax> statements, MethodDeclarationSyntax testMethod, int spaces)
        {
            var lambda = Argument(ParenthesizedLambda(Block(statements, spaces)));

            var catchClauseDeclaration = catchClause.Declaration;
            var exceptionType = catchClauseDeclaration?.Type ?? SyntaxFactory.ParseTypeName(nameof(Exception));
            var exceptionIdentifier = catchClauseDeclaration.GetName();

            var useAwait = testMethod.Modifiers.Any(SyntaxKind.AsyncKeyword) || tryStatement.Block.Statements.Any(SyntaxKind.ThrowStatement);

            ExpressionSyntax expression = Invocation("Assert", useAwait ? "ThrowsAsync" : "Throws", exceptionType, lambda);

            if (useAwait)
            {
                expression = SyntaxFactory.AwaitExpression(expression);
            }

            if (exceptionIdentifier.IsNullOrEmpty())
            {
                return SyntaxFactory.ExpressionStatement(expression).WithTriviaFrom(tryStatement);
            }

            var declarator = SyntaxFactory.VariableDeclarator(exceptionIdentifier).WithInitializer(SyntaxFactory.EqualsValueClause(expression));
            var declaration = SyntaxFactory.VariableDeclaration(exceptionType, declarator.ToSeparatedSyntaxList());
            var localDeclaration = SyntaxFactory.LocalDeclarationStatement(declaration);

            return localDeclaration.WithLeadingTriviaFrom(tryStatement);
        }

        private static SyntaxNode GetUpdatedSyntaxRootForNUnit(SyntaxNode root, TryStatementSyntax tryStatement)
        {
            var spaces = tryStatement.GetPositionWithinStartLine();

            var catchClauses = tryStatement.Catches;

            var exceptionIdentifiers = catchClauses.ToHashSet(_ => _.Declaration?.Identifier.ToString());

            // we have a finally block, so we need to ensure that the catch statements are added to the try block
            var additionalStatements = catchClauses.SelectMany(_ => _.Block.Statements).ToList();

            additionalStatements.RemoveAll(IsAssertFail);

            // remove all that are assertions on the exceptions inside the catch blocks as we create our own assertion
            additionalStatements.RemoveAll(_ => IsAssert(_) && _.DescendantTokens().Any(__ => exceptionIdentifiers.Contains(__.ToString())));

            var statements = tryStatement.Block.Statements;

            var assertion = tryStatement.Finally is null
                            ? CreateNUnitAssertionStatement(statements, catchClauses, spaces + Constants.Indentation).WithTriviaFrom(tryStatement)
                            : CreateNUnitAssertionStatement(statements, catchClauses, spaces + Constants.Indentation + Constants.Indentation);

            additionalStatements.Insert(0, assertion);

            if (additionalStatements.Skip(1).Any(IsAssert))
            {
                // ensure that the first additional statement is separated by an empty line
                additionalStatements[1] = additionalStatements[1].WithLeadingEmptyLine();
            }

            return tryStatement.Finally is null
                   ? root.ReplaceNode(tryStatement, additionalStatements.Select(_ => _.WithLeadingSpaces(spaces)))
                   : root.ReplaceNode(tryStatement, tryStatement.Without(catchClauses).WithBlock(SyntaxFactory.Block(additionalStatements)));
        }

        private static ExpressionStatementSyntax CreateNUnitAssertionStatement(SyntaxList<StatementSyntax> statements, SyntaxList<CatchClauseSyntax> catchClauses, in int spaces)
        {
            var expressionStatements = statements.OfType<ExpressionStatementSyntax>().ToList();
            var assertFailStatements = expressionStatements.Where(IsAssertFail).ToList();

            var arguments = new List<ArgumentSyntax>
                            {
                                Argument(ParenthesizedLambda(Block(statements.Except(assertFailStatements), spaces))),
                                ThrowsArgument(catchClauses, assertFailStatements).WithLeadingSpace(),
                            };

            if (assertFailStatements.Count > 0 && assertFailStatements[0].Expression is InvocationExpressionSyntax i && i.ArgumentList?.Arguments is SeparatedSyntaxList<ArgumentSyntax> args && args.Count > 0)
            {
                arguments.Add(args[0].WithLeadingSpace());
            }

            return SyntaxFactory.ExpressionStatement(AssertThat(arguments.ToArray()));
        }

        private static BlockSyntax Block(IEnumerable<StatementSyntax> statements, int spaces)
        {
            return SyntaxFactory.Block(
                                   SyntaxKind.OpenBraceToken.AsToken().WithEndOfLine(),
                                   statements.Select(_ => _.WithLeadingSpaces(spaces)).ToSyntaxList(),
                                   SyntaxKind.CloseBraceToken.AsToken());
        }

        private static ArgumentSyntax ThrowsArgument(in SyntaxList<CatchClauseSyntax> catches, List<ExpressionStatementSyntax> assertFailStatements)
        {
            var needsException = assertFailStatements.Count > 0;

            if (catches.Count > 0)
            {
                var catchClause = catches[0];

                if (catchClause.Declaration is CatchDeclarationSyntax catchDeclaration)
                {
                    var statements = catchClause.Block.Statements;

                    if (statements.Count is 0 && needsException is false)
                    {
                        return Throws("Nothing");
                    }

                    var asserts = statements.Where(IsAssert).ToList();

                    if (asserts.Count is 0)
                    {
                        return Throws(catchDeclaration.Type);
                    }

                    return Throws(catchDeclaration, asserts);
                }
            }

            return Throws(needsException ? "Exception" : "Nothing");
        }

        private static ArgumentSyntax Throws(CatchDeclarationSyntax catchDeclaration, IEnumerable<StatementSyntax> asserts)
        {
            var exceptionType = catchDeclaration.Type;
            var exceptionIdentifier = catchDeclaration.GetName();

            var exceptionSpecificAsserts = asserts.OfType<ExpressionStatementSyntax>().Where(_ => _.DescendantTokens().Any(__ => __.ToString() == exceptionIdentifier)).ToList();

            // check for assertions in catch block, and use those
            if (exceptionSpecificAsserts.Count > 0)
            {
                ExpressionSyntax expression = Invocation("Throws", "TypeOf", exceptionType);

                var continuation = "With";

                foreach (var assert in exceptionSpecificAsserts)
                {
                    if (assert.Expression is InvocationExpressionSyntax i)
                    {
                        if (i.GetName() is "That")
                        {
                            var arguments = i.ArgumentList.Arguments;

                            if (arguments.Count > 1)
                            {
                                var argument = arguments[0];

                                if (argument.Expression is MemberAccessExpressionSyntax maes && maes.GetIdentifierName() == exceptionIdentifier)
                                {
                                    var name = maes.GetName();

                                    if (name is "InnerException")
                                    {
                                        expression = Member(expression, continuation, "InnerException");
                                    }
                                    else
                                    {
                                        expression = Invocation(Member(expression, continuation), "Property", Argument(NameOf(exceptionType, name)));
                                    }

                                    // get invocation from other argument
                                    var other = arguments.FirstOrDefault(_ => _.GetName() != name);

                                    if (other?.Expression is InvocationExpressionSyntax otherInvocation)
                                    {
                                        expression = Invocation(expression, otherInvocation.GetName(), otherInvocation.ArgumentList.Arguments.ToArray());
                                    }

                                    continuation = "And";
                                }
                            }
                        }
                    }
                }

                return Argument(expression);
            }

            return Throws(exceptionType);
        }
    }
}