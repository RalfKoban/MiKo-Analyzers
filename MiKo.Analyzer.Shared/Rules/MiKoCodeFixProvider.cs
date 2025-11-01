using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules
{
    public abstract class MiKoCodeFixProvider : CodeFixProvider
    {
        private readonly string m_equivalenceKey;

        protected MiKoCodeFixProvider() => m_equivalenceKey = string.Intern(GetType().Name); // ncrunch: no coverage

        public abstract string FixableDiagnosticId { get; }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FixableDiagnosticId);

        protected virtual string Title => Resources.ResourceManager.GetString(FixableDiagnosticId + "_CodeFixTitle", Resources.Culture);

//// ncrunch: rdi off

        protected virtual bool IsSolutionWide => false;

        protected virtual bool IsTrivia => false;

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostics = context.Diagnostics;

            if (diagnostics.Length > 0 && IsApplicable(diagnostics))
            {
                var diagnostic = diagnostics[0];

                var document = context.Document;

                var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var codeFix = CreateCodeFix(document, root, diagnostic);

                if (codeFix != null)
                {
                    context.RegisterCodeFix(codeFix, diagnostic);
                }
            }
        }

//// ncrunch: rdi default

        /// <summary>
        /// Gets an argument list containing the specified arguments.
        /// </summary>
        /// <param name="arguments">
        /// The arguments to include in the argument list.
        /// </param>
        /// <returns>
        /// The argument list containing the specified arguments.
        /// </returns>
        protected static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments) => SyntaxFactory.ArgumentList(arguments.ToSeparatedSyntaxList());

        /// <summary>
        /// Gets an invocation expression for a method call on the specified expression.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method is invoked.
        /// </param>
        /// <param name="name">
        /// The name of the method to invoke.
        /// </param>
        /// <returns>
        /// The invocation expression representing the method call.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name)
        {
            // that's for the method call
            var member = Member(expression, name);

            return Invocation(member);
        }

        /// <summary>
        /// Gets an invocation expression for a method call on the specified expression with a single argument.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method is invoked.
        /// </param>
        /// <param name="name">
        /// The name of the method to invoke.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the method call with the specified argument.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, ArgumentSyntax argument)
        {
            // that's for the method call
            var member = Member(expression, name);

            return Invocation(member, argument);
        }

        /// <summary>
        /// Gets an invocation expression for a method call on the specified expression with multiple arguments.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method is invoked.
        /// </param>
        /// <param name="name">
        /// The name of the method to invoke.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the method call with the specified arguments.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = Member(expression, name);

            return Invocation(member, arguments);
        }

        /// <summary>
        /// Gets an invocation expression for a generic method call on the specified expression with a single type argument.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method is invoked.
        /// </param>
        /// <param name="name">
        /// The name of the generic method to invoke.
        /// </param>
        /// <param name="item">
        /// The type argument for the generic method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the generic method call.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, TypeSyntax item)
        {
            // that's for the method call
            var member = Member(expression, name, item);

            return Invocation(member);
        }

        /// <summary>
        /// Gets an invocation expression for a generic method call on the specified expression with multiple type arguments.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method is invoked.
        /// </param>
        /// <param name="name">
        /// The name of the generic method to invoke.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the generic method call.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, TypeSyntax[] items)
        {
            // that's for the method call
            var member = Member(expression, name, items);

            return Invocation(member);
        }

        /// <summary>
        /// Gets an invocation expression for a method call with the specified arguments.
        /// </summary>
        /// <param name="member">
        /// The member expression representing the method to invoke.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the method call with the specified arguments.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax member, params ArgumentSyntax[] arguments)
        {
            // that's for the argument
            var argumentList = ArgumentList(arguments);

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, argumentList);
        }

        /// <summary>
        /// Gets an invocation expression for a generic method call on a type with multiple type arguments.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type containing the method.
        /// </param>
        /// <param name="methodName">
        /// The name of the generic method to invoke.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the generic method call.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = Member(typeName, methodName, items);

            return Invocation(member);
        }

        /// <summary>
        /// Gets an invocation expression for a nested method call on the specified expression with multiple member names and type arguments.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method chain is invoked.
        /// </param>
        /// <param name="names">
        /// The names of the members in the method chain, with the last name being the method to invoke.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the nested method call.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string[] names, params TypeSyntax[] items)
        {
            var length = names.Length;

            switch (length)
            {
                case 0: return Invocation(expression);
                case 1: return Invocation(expression, names[0], items);
                case 2: return Invocation(expression, names[0], names[1], items);
                default:
                {
                    var member = Member(expression, names.Take(length - 1).ToArray());
                    var method = Member(member, names[length - 1], items);

                    return Invocation(method);
                }
            }
        }

        /// <summary>
        /// Gets an invocation expression for a chained method call with two member names and multiple arguments.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method chain is invoked.
        /// </param>
        /// <param name="name1">
        /// The name of the first member in the chain.
        /// </param>
        /// <param name="name2">
        /// The name of the second member (the method) to invoke.
        /// </param>
        /// <param name="arguments">
        /// The arguments to pass to the method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the chained method call with the specified arguments.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name1, string name2, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = Member(Member(expression, name1), name2);

            return Invocation(member, arguments);
        }

        /// <summary>
        /// Gets an invocation expression for a chained generic method call with two member names and multiple type arguments.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the method chain is invoked.
        /// </param>
        /// <param name="name1">
        /// The name of the first member in the chain.
        /// </param>
        /// <param name="name2">
        /// The name of the second member (the generic method) to invoke.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic method.
        /// </param>
        /// <returns>
        /// The invocation expression representing the chained generic method call.
        /// </returns>
        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name1, string name2, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = Member(Member(expression, name1), name2, items);

            return Invocation(member);
        }

        /// <summary>
        /// Gets an is-pattern expression that represents the negation of the specified pattern expression.
        /// </summary>
        /// <param name="syntax">
        /// The is-pattern expression to negate.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the negated pattern.
        /// </returns>
        protected static IsPatternExpressionSyntax IsNotPattern(IsPatternExpressionSyntax syntax) => IsNotPattern(syntax.Expression, syntax.Pattern);

        /// <summary>
        /// Gets an is-pattern expression that represents the negation of the specified pattern against an operand.
        /// </summary>
        /// <param name="operand">
        /// The expression to test against the pattern.
        /// </param>
        /// <param name="pattern">
        /// The pattern to negate.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the negated pattern test.
        /// </returns>
        protected static IsPatternExpressionSyntax IsNotPattern(ExpressionSyntax operand, PatternSyntax pattern) => IsPattern(operand, SyntaxFactory.UnaryPattern(pattern));

        /// <summary>
        /// Gets an is-pattern expression that tests an operand against the specified pattern.
        /// </summary>
        /// <param name="operand">
        /// The expression to test against the pattern.
        /// </param>
        /// <param name="pattern">
        /// The pattern to test.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the pattern test.
        /// </returns>
        protected static IsPatternExpressionSyntax IsPattern(ExpressionSyntax operand, PatternSyntax pattern) => SyntaxFactory.IsPatternExpression(operand, pattern);

        /// <summary>
        /// Gets an is-pattern expression that tests an operand against a constant expression pattern.
        /// </summary>
        /// <param name="operand">
        /// The expression to test against the pattern.
        /// </param>
        /// <param name="expression">
        /// The constant expression to use as the pattern.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the constant pattern test.
        /// </returns>
        protected static IsPatternExpressionSyntax IsPattern(ExpressionSyntax operand, ExpressionSyntax expression) => IsPattern(operand, SyntaxFactory.ConstantPattern(expression));

        /// <summary>
        /// Gets an is-pattern expression that tests whether an operand is <see langword="false"/>.
        /// </summary>
        /// <param name="operand">
        /// The expression to test.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the test for <see langword="false"/>.
        /// </returns>
        protected static IsPatternExpressionSyntax IsFalsePattern(ExpressionSyntax operand) => IsPattern(operand, FalseLiteral());

        /// <summary>
        /// Gets an is-pattern expression that tests whether an operand is <see langword="true"/>.
        /// </summary>
        /// <param name="operand">
        /// The expression to test.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the test for <see langword="true"/>.
        /// </returns>
        protected static IsPatternExpressionSyntax IsTruePattern(ExpressionSyntax operand) => IsPattern(operand, TrueLiteral());

        /// <summary>
        /// Gets an is-pattern expression that tests whether an operand is <see langword="null"/>.
        /// </summary>
        /// <param name="operand">
        /// The expression to test.
        /// </param>
        /// <returns>
        /// The is-pattern expression representing the test for <see langword="null"/>.
        /// </returns>
        protected static IsPatternExpressionSyntax IsNullPattern(ExpressionSyntax operand) => IsPattern(operand, NullLiteral());

        /// <summary>
        /// Gets a literal expression of the specified kind.
        /// </summary>
        /// <param name="expressionKind">
        /// One of the enumeration members that specifies the kind of literal expression to create.
        /// </param>
        /// <returns>
        /// The literal expression of the specified kind.
        /// </returns>
        protected static LiteralExpressionSyntax Literal(in SyntaxKind expressionKind) => SyntaxFactory.LiteralExpression(expressionKind);

        /// <summary>
        /// Gets a literal expression representing <see langword="null"/>.
        /// </summary>
        /// <returns>
        /// The literal expression representing <see langword="null"/>.
        /// </returns>
        protected static LiteralExpressionSyntax NullLiteral() => Literal(SyntaxKind.NullLiteralExpression);

        /// <summary>
        /// Gets a literal expression representing <see langword="false"/>.
        /// </summary>
        /// <returns>
        /// The literal expression representing <see langword="false"/>.
        /// </returns>
        protected static LiteralExpressionSyntax FalseLiteral() => Literal(SyntaxKind.FalseLiteralExpression);

        /// <summary>
        /// Gets a literal expression representing <see langword="true"/>.
        /// </summary>
        /// <returns>
        /// The literal expression representing <see langword="true"/>.
        /// </returns>
        protected static LiteralExpressionSyntax TrueLiteral() => Literal(SyntaxKind.TrueLiteralExpression);

        /// <summary>
        /// Gets a predefined type syntax for the specified syntax kind.
        /// </summary>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind of the predefined type.
        /// </param>
        /// <returns>
        /// The predefined type syntax for the specified kind.
        /// </returns>
        protected static PredefinedTypeSyntax PredefinedType(in SyntaxKind kind) => SyntaxFactory.PredefinedType(kind.AsToken());

        /// <summary>
        /// Gets a conditional access expression for accessing a member on an expression.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the conditional access is performed.
        /// </param>
        /// <param name="name">
        /// The name of the member to access conditionally.
        /// </param>
        /// <returns>
        /// The conditional access expression representing the member access.
        /// </returns>
        protected static ConditionalAccessExpressionSyntax ConditionalAccess(ExpressionSyntax expression, string name) => ConditionalAccess(expression, IdentifierName(name));

        /// <summary>
        /// Gets a conditional access expression for accessing a member on an expression.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the conditional access is performed.
        /// </param>
        /// <param name="binding">
        /// The member binding to access conditionally.
        /// </param>
        /// <returns>
        /// The conditional access expression representing the member access.
        /// </returns>
        protected static ConditionalAccessExpressionSyntax ConditionalAccess(ExpressionSyntax expression, SimpleNameSyntax binding)
        {
            return SyntaxFactory.ConditionalAccessExpression(expression, SyntaxFactory.MemberBindingExpression(binding));
        }

        /// <summary>
        /// Gets a member access expression for accessing a named member on an expression.
        /// </summary>
        /// <param name="expression">
        /// The expression on which the member is accessed.
        /// </param>
        /// <param name="name">
        /// The name of the member to access.
        /// </param>
        /// <returns>
        /// The member access expression representing the member access.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        /// <summary>
        /// Gets a member access expression for accessing a named member on an expression.
        /// </summary>
        /// <param name="syntax">
        /// The expression on which the member is accessed.
        /// </param>
        /// <param name="name">
        /// The name of the member to access.
        /// </param>
        /// <returns>
        /// The member access expression representing the member access.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax syntax, string name) => Member(syntax, IdentifierName(name));

        /// <summary>
        /// Gets a member access expression for accessing a chain of named members on an expression.
        /// </summary>
        /// <param name="syntax">
        /// The expression on which the member chain is accessed.
        /// </param>
        /// <param name="names">
        /// The names of the members to access in sequence.
        /// </param>
        /// <returns>
        /// The member access expression representing the chained member access.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax syntax, params string[] names)
        {
            var start = Member(syntax, IdentifierName(names[0]));

            var result = names.Skip(1).Aggregate(start, Member);

            return result;
        }

        /// <summary>
        /// Gets a member access expression for accessing a potentially generic member on an expression.
        /// </summary>
        /// <param name="syntax">
        /// The expression on which the member is accessed.
        /// </param>
        /// <param name="name">
        /// The name of the member to access.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic member, or an empty array for non-generic members.
        /// </param>
        /// <returns>
        /// The member access expression representing the member access.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax syntax, string name, params TypeSyntax[] items)
        {
            if (items is null || items.Length is 0)
            {
                return Member(syntax, IdentifierName(name));
            }

            return Member(syntax, GenericName(name, items));
        }

        /// <summary>
        /// Gets a member access expression for accessing a generic method on a type.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type containing the method.
        /// </param>
        /// <param name="methodName">
        /// The name of the generic method to access.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic method.
        /// </param>
        /// <returns>
        /// The member access expression representing the generic method access.
        /// </returns>
        protected static MemberAccessExpressionSyntax Member(string typeName, string methodName, params TypeSyntax[] items)
        {
            var type = IdentifierName(typeName);
            var method = GenericName(methodName, items);

            return Member(type, method);
        }

        /// <summary>
        /// Gets an identifier name syntax for the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the identifier.
        /// </param>
        /// <returns>
        /// The identifier name syntax representing the specified name.
        /// </returns>
        protected static IdentifierNameSyntax IdentifierName(string name) => SyntaxFactory.IdentifierName(name);

        /// <summary>
        /// Gets an identifier name syntax for the specified token.
        /// </summary>
        /// <param name="token">
        /// The token representing the identifier.
        /// </param>
        /// <returns>
        /// The identifier name syntax representing the specified token.
        /// </returns>
        protected static IdentifierNameSyntax IdentifierName(in SyntaxToken token) => SyntaxFactory.IdentifierName(token);

        /// <summary>
        /// Gets a generic name syntax with the specified name and type arguments.
        /// </summary>
        /// <param name="name">
        /// The name of the generic type or method.
        /// </param>
        /// <param name="items">
        /// The type arguments for the generic name.
        /// </param>
        /// <returns>
        /// The generic name syntax with the specified type arguments.
        /// </returns>
        protected static GenericNameSyntax GenericName(string name, TypeSyntax[] items) => SyntaxFactory.GenericName(name).AddTypeArgumentListArguments(items);

        /// <summary>
        /// Gets a generic name syntax with the specified name and type argument list.
        /// </summary>
        /// <param name="name">
        /// The name of the generic type or method.
        /// </param>
        /// <param name="types">
        /// The type argument list for the generic name.
        /// </param>
        /// <returns>
        /// The generic name syntax with the specified type argument list.
        /// </returns>
        protected static GenericNameSyntax GenericName(string name, TypeArgumentListSyntax types) => SyntaxFactory.GenericName(name).WithTypeArgumentList(types);

        /// <summary>
        /// Gets a syntax node with leading spaces adjusted to the specified position, including proper indentation for descendant nodes on new lines.
        /// </summary>
        /// <typeparam name="TSyntaxNode">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="syntaxNode">
        /// The syntax node to adjust.
        /// </param>
        /// <param name="spaces">
        /// The number of leading spaces to apply.
        /// </param>
        /// <returns>
        /// The syntax node with adjusted leading spaces.
        /// </returns>
        protected static TSyntaxNode GetSyntaxWithLeadingSpaces<TSyntaxNode>(TSyntaxNode syntaxNode, in int spaces) where TSyntaxNode : SyntaxNode
        {
            var syntax = syntaxNode.WithLeadingSpaces(spaces);

            // collect all descendant nodes that are the first ones starting on a new line, then adjust leading space for each of those
            var startingNodes = GetNodesAndTokensStartingOnSeparateLines(syntax).ToList();

            if (startingNodes.Count is 0)
            {
                return syntax;
            }

            var additionalSpaces = syntax.GetPositionWithinStartLine() - syntaxNode.GetPositionWithinStartLine();

            return syntax.WithAdditionalLeadingSpacesOnDescendants(startingNodes, additionalSpaces);
        }

        /// <summary>
        /// Gets a statement with leading spaces adjusted to the specified position.
        /// </summary>
        /// <param name="statement">
        /// The statement to adjust.
        /// </param>
        /// <param name="spaces">
        /// The number of leading spaces to apply.
        /// </param>
        /// <returns>
        /// The statement with adjusted leading spaces.
        /// </returns>
        protected static StatementSyntax GetUpdatedStatement(StatementSyntax statement, in int spaces) => GetSyntaxWithLeadingSpaces(statement, spaces);

        /// <summary>
        /// Gets a block with properly indented braces and statements.
        /// </summary>
        /// <param name="block">
        /// The block to adjust.
        /// </param>
        /// <param name="spaces">
        /// The number of leading spaces to apply to the braces.
        /// </param>
        /// <returns>
        /// The block with properly indented braces and statements, or <see langword="null"/> if the input block is <see langword="null"/>.
        /// </returns>
        protected static BlockSyntax GetUpdatedBlock(BlockSyntax block, in int spaces)
        {
            if (block is null)
            {
                return null;
            }

            var indentation = spaces + Constants.Indentation;

            return block.WithOpenBraceToken(block.OpenBraceToken.WithLeadingSpaces(spaces))
                        .WithStatements(block.Statements.Select(_ => GetUpdatedStatement(_, indentation)).ToSyntaxList())
                        .WithCloseBraceToken(block.CloseBraceToken.WithLeadingSpaces(spaces));
        }

//// ncrunch: rdi off

        protected virtual bool IsApplicable(in ImmutableArray<Diagnostic> issues) => issues.Any();

        protected virtual SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected virtual SyntaxToken GetToken(in SyntaxTrivia trivia, Diagnostic issue) => trivia.Token;

        protected virtual SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxToken GetUpdatedToken(in SyntaxToken token, Diagnostic issue) => token;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, in SyntaxTrivia trivia, Diagnostic issue) => null;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue) => root.WithoutAnnotations(annotationOfSyntax);

        protected virtual Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken) => Task.FromResult(document.Project.Solution);

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var updatedSyntax = GetUpdatedSyntax(document, syntax, issue);

            var newRoot = root;

            var annotation = new SyntaxAnnotation("document adjustment");

            if (ReferenceEquals(updatedSyntax, syntax) is false)
            {
                if (updatedSyntax is null)
                {
                    newRoot = root.Without(syntax);
                }
                else
                {
                    newRoot = root.ReplaceNode(syntax, updatedSyntax.WithAnnotation(annotation));

                    // let's see if the node now gets parents when we annotate it
                    updatedSyntax = newRoot.GetAnnotatedNodes(annotation).First();
                }

                if (newRoot is null)
                {
                    return Task.FromResult(document);
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var finalRoot = GetUpdatedSyntaxRoot(document, newRoot, updatedSyntax, annotation, issue) ?? newRoot;
            var newDocument = document.WithSyntaxRoot(finalRoot);

            return Task.FromResult(newDocument);
        }

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxTrivia trivia, Diagnostic issue, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var oldToken = GetToken(trivia, issue);
            var updatedToken = GetUpdatedToken(oldToken, issue);

            var newRoot = oldToken == updatedToken ? root : root.ReplaceToken(oldToken, updatedToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var finalRoot = GetUpdatedSyntaxRoot(document, newRoot, trivia, issue) ?? newRoot;
            var newDocument = document.WithSyntaxRoot(finalRoot);

            return Task.FromResult(newDocument);
        }

        private static IEnumerable<SyntaxNodeOrToken> GetNodesAndTokensStartingOnSeparateLines(SyntaxNode startingNode)
        {
            var currentLine = startingNode.GetStartingLine();

            foreach (var nodeOrToken in startingNode.AllDescendantNodesAndTokens())
            {
                var startingLine = nodeOrToken.GetStartingLine();

                if (startingLine != currentLine)
                {
                    currentLine = startingLine;

                    if (nodeOrToken.IsToken)
                    {
                        var token = nodeOrToken.AsToken();

                        switch (token.Kind())
                        {
                            case SyntaxKind.PlusToken when token.Parent.IsStringCreation():
                            {
                                // ignore string constructions via add
                                continue;
                            }

                            case SyntaxKind.CloseParenToken when token.Parent is ArgumentListSyntax l && l.Arguments.Last().Expression.IsStringCreation():
                            {
                                // ignore string constructions via add
                                continue;
                            }
                        }
                    }

                    yield return nodeOrToken;
                }
            }
        }

        private CodeAction CreateCodeFix(Document document, SyntaxNode root, Diagnostic issue)
        {
            var issueSpan = issue.Location.SourceSpan;
            var startPosition = issueSpan.Start;

            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, cancellationToken => ApplyDocumentCodeFixAsync(document, root, trivia, issue, cancellationToken), m_equivalenceKey); // ncrunch: no coverage
            }

            // TODO RKN
            // var token = root.FindToken(startPosition);
            // var syntaxNodes = token.Parent.AncestorsAndSelf();
            var node = root.FindNode(issueSpan, true, true);
            var syntaxNodes = node.AncestorsAndSelf();

            var syntax = GetSyntax(syntaxNodes);

            if (syntax is null)
            {
                return null;
            }

            if (IsSolutionWide)
            {
                return CodeAction.Create(Title, cancellationToken => ApplySolutionCodeFixAsync(document, root, syntax, issue, cancellationToken), m_equivalenceKey); // ncrunch: no coverage
            }

            return CodeAction.Create(Title, cancellationToken => ApplyDocumentCodeFixAsync(document, root, syntax, issue, cancellationToken), m_equivalenceKey); // ncrunch: no coverage
        }

//// ncrunch: rdi default
    }
}