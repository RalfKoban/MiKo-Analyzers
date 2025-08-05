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
            var assertFails = statements.Where(IsAssertFail).ToList();
            var nonAssertFails = statements.Except(assertFails).ToList();

            var catches = tryStatement.Catches;

            if (catches.Count > 0)
            {
                var additionalStatements = catches.SelectMany(_ => _.Block.Statements).ToList();

                if (additionalStatements.Any(IsAssertFail))
                {
                    return nonAssertFails;
                }

                if (additionalStatements.Any(IsAssert))
                {
                    var assertionStatement = CreateXunitAssertionStatement(tryStatement, catches[0], nonAssertFails, testMethod, spaces);

                    additionalStatements.Insert(0, assertionStatement);

                    // ensure that the first additional statement is separated by an empty line
                    additionalStatements[1] = additionalStatements[1].WithLeadingEmptyLine();

                    return additionalStatements;
                }

                if (assertFails.Count > 0)
                {
                    var assertionStatement = CreateXunitAssertionStatement(tryStatement, catches[0], nonAssertFails, testMethod, spaces);

                    return new[] { assertionStatement };
                }

                nonAssertFails.AddRange(additionalStatements);
            }

            return nonAssertFails;
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

            var assertion = tryStatement.Finally is null
                            ? CreateNUnitAssertionStatement(tryStatement, catchClauses, spaces + Constants.Indentation).WithTriviaFrom(tryStatement)
                            : CreateNUnitAssertionStatement(tryStatement, catchClauses, spaces + Constants.Indentation + Constants.Indentation);

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

        private static ExpressionStatementSyntax CreateNUnitAssertionStatement(TryStatementSyntax tryStatement, in SyntaxList<CatchClauseSyntax> catchClauses, in int spaces)
        {
            var statements = tryStatement.Block.Statements;
            var tryAssertFails = statements.OfType<ExpressionStatementSyntax>().Where(IsAssertFail).ToList();

            var arguments = new List<ArgumentSyntax>
                            {
                                Argument(ParenthesizedLambda(Block(statements.Except(tryAssertFails), spaces))),
                                Throws(catchClauses, tryAssertFails).WithLeadingSpace(),
                            };

            if (tryAssertFails.Count > 0)
            {
                if (tryAssertFails[0].Expression is InvocationExpressionSyntax i && i.ArgumentList?.Arguments is SeparatedSyntaxList<ArgumentSyntax> args && args.Count > 0)
                {
                    arguments.AddRange(args.Select(_ => _.WithLeadingSpace()));
                }
            }
            else
            {
                // maybe we have an 'Assert.Fail' in the catch block, so we need to check those
                var catchAssertFail = catchClauses.SelectMany(_ => _.Block.Statements).OfType<ExpressionStatementSyntax>().FirstOrDefault(IsAssertFail);

                if (catchAssertFail?.Expression is InvocationExpressionSyntax i && i.ArgumentList?.Arguments is SeparatedSyntaxList<ArgumentSyntax> args && args.Count > 0)
                {
                    arguments.AddRange(args.Select(_ => _.WithLeadingSpace()));
                }
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

        private static ArgumentSyntax Throws(in SyntaxList<CatchClauseSyntax> catches, List<ExpressionStatementSyntax> tryAssertFails)
        {
            var needsException = tryAssertFails.Count > 0;

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

                    if (statements.Any(IsAssertFail))
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
                if (exceptionSpecificAsserts.All(_ => _.Expression.GetName() is "That"))
                {
                    return ThrowsForNUnitThat(exceptionSpecificAsserts, exceptionType, exceptionIdentifier);
                }

                // we have assertions that are not 'That', so we need to handle those differently
                return ThrowsForNUnitClassic(exceptionType, exceptionIdentifier, exceptionSpecificAsserts);
            }

            return Throws(exceptionType);
        }

        private static ArgumentSyntax ThrowsForNUnitThat(IEnumerable<ExpressionStatementSyntax> exceptionSpecificAsserts, TypeSyntax exceptionType, string exceptionIdentifier)
        {
            var invocation = Invocation("Throws", "TypeOf", exceptionType);

            var continuation = "With";

            foreach (var assert in exceptionSpecificAsserts)
            {
                if (assert.Expression is InvocationExpressionSyntax i && i.ArgumentList is ArgumentListSyntax argumentList)
                {
                    invocation = InvocationForNUnitThat(continuation, invocation, argumentList.Arguments, 0, exceptionType, exceptionIdentifier);
                }

                continuation = "And";
            }

            return Argument(invocation);
        }

        private static InvocationExpressionSyntax InvocationForNUnitThat(string continuation, InvocationExpressionSyntax invocationToBuild, in SeparatedSyntaxList<ArgumentSyntax> arguments, in int argumentIndex, TypeSyntax exceptionType, string exceptionIdentifier)
        {
            var argument = arguments[argumentIndex];

            if (argument.Expression is MemberAccessExpressionSyntax maes && maes.GetIdentifierName() == exceptionIdentifier)
            {
                var name = maes.GetName();
                var nested = NestedExpression(invocationToBuild, continuation, exceptionType, name);

                // get invocation from other argument
                var other = arguments.FirstOrDefault(_ => _.GetName() != name);

                if (other?.Expression is InvocationExpressionSyntax otherInvocation)
                {
                    return Invocation(nested, otherInvocation.GetNames(), otherInvocation.GetTypes()).WithArgumentList(otherInvocation.ArgumentList);
                }
            }

            return invocationToBuild;
        }

        private static ArgumentSyntax ThrowsForNUnitClassic(TypeSyntax exceptionType, string exceptionIdentifier, List<ExpressionStatementSyntax> exceptionSpecificAsserts)
        {
            var invocation = Invocation("Throws", "TypeOf", exceptionType);

            var continuation = "With";

            // we have assertions that are not 'That', so we need to handle those differently
            foreach (var assert in exceptionSpecificAsserts)
            {
                if (assert.Expression is InvocationExpressionSyntax i)
                {
                    invocation = InvocationForNUnitClassic(invocation, continuation, i, exceptionType, exceptionIdentifier);
                }

                continuation = "And";
            }

            return Argument(invocation);
        }

        private static InvocationExpressionSyntax InvocationForNUnitClassic(InvocationExpressionSyntax invocationToBuild, string continuation, InvocationExpressionSyntax original, TypeSyntax exceptionType, string exceptionIdentifier)
        {
            var arguments = original.ArgumentList.Arguments;

            if (arguments.Count > 1)
            {
                var argument1 = arguments[1];

                if (argument1.Expression.GetIdentifierName() == exceptionIdentifier)
                {
                    return InvocationForNUnitClassic(invocationToBuild, continuation, original, exceptionType, argument1);
                }
            }

            // maybe swapped arguments, so check them
            var argument0 = arguments[0];

            if (argument0.Expression.GetIdentifierName() == exceptionIdentifier)
            {
                return InvocationForNUnitClassic(invocationToBuild, continuation, original, exceptionType, argument0);
            }

            return invocationToBuild;
        }

        private static InvocationExpressionSyntax InvocationForNUnitClassic(InvocationExpressionSyntax invocationToBuild, string continuation, InvocationExpressionSyntax original, TypeSyntax exceptionType, ArgumentSyntax argument)
        {
            var name = argument.GetName();
            var nested = NestedExpression(invocationToBuild, continuation, exceptionType, name);

            // get invocation from other argument
            var arguments = original.ArgumentList.Arguments;
            var other = arguments.FirstOrDefault(_ => _.GetName() != name);

            // get types
            var types = original.GetTypes();
            var assertionMethod = original.GetName();

            switch (assertionMethod)
            {
                case "AreEqual": return Invocation(nested, "EqualTo", types).WithArgument(other);
                case "AreNotEqual": return Invocation(nested, "Not", "EqualTo", types).WithArgument(other);
                case "IsInstanceOf": return Invocation(nested, "InstanceOf", types).WithArgument(other);
                case "IsNotInstanceOf": return Invocation(nested, "Not", "InstanceOf", types).WithArgument(other);
            }

            return invocationToBuild;
        }

        private static ExpressionSyntax NestedExpression(InvocationExpressionSyntax invocationToBuild, string continuation, TypeSyntax exceptionType, string propertyName)
        {
            if (propertyName is "InnerException")
            {
                return Member(invocationToBuild, continuation, "InnerException");
            }

            return Invocation(invocationToBuild, continuation, "Property", Argument(NameOf(exceptionType, propertyName)));
        }
    }
}