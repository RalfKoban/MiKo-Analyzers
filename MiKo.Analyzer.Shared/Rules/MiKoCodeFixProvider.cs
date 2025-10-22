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

        protected static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments) => SyntaxFactory.ArgumentList(arguments.ToSeparatedSyntaxList());

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name)
        {
            // that's for the method call
            var member = Member(expression, name);

            return Invocation(member);
        }

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, ArgumentSyntax argument)
        {
            // that's for the method call
            var member = Member(expression, name);

            return Invocation(member, argument);
        }

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = Member(expression, name);

            return Invocation(member, arguments);
        }

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, TypeSyntax item)
        {
            // that's for the method call
            var member = Member(expression, name, item);

            return Invocation(member);
        }

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name, TypeSyntax[] items)
        {
            // that's for the method call
            var member = Member(expression, name, items);

            return Invocation(member);
        }

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax member, params ArgumentSyntax[] arguments)
        {
            // that's for the argument
            var argumentList = ArgumentList(arguments);

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, argumentList);
        }

        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = Member(typeName, methodName, items);

            return Invocation(member);
        }

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

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name1, string name2, params ArgumentSyntax[] arguments)
        {
            // that's for the method call
            var member = Member(Member(expression, name1), name2);

            return Invocation(member, arguments);
        }

        protected static InvocationExpressionSyntax Invocation(ExpressionSyntax expression, string name1, string name2, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = Member(Member(expression, name1), name2, items);

            return Invocation(member);
        }

        protected static IsPatternExpressionSyntax IsNotPattern(IsPatternExpressionSyntax syntax) => IsNotPattern(syntax.Expression, syntax.Pattern);

        protected static IsPatternExpressionSyntax IsNotPattern(ExpressionSyntax operand, PatternSyntax pattern) => IsPattern(operand, SyntaxFactory.UnaryPattern(pattern));

        protected static IsPatternExpressionSyntax IsPattern(ExpressionSyntax operand, PatternSyntax pattern) => SyntaxFactory.IsPatternExpression(operand, pattern);

        protected static IsPatternExpressionSyntax IsPattern(ExpressionSyntax operand, ExpressionSyntax expression) => IsPattern(operand, SyntaxFactory.ConstantPattern(expression));

        protected static IsPatternExpressionSyntax IsFalsePattern(ExpressionSyntax operand) => IsPattern(operand, FalseLiteral());

        protected static IsPatternExpressionSyntax IsTruePattern(ExpressionSyntax operand) => IsPattern(operand, TrueLiteral());

        protected static IsPatternExpressionSyntax IsNullPattern(ExpressionSyntax operand) => IsPattern(operand, NullLiteral());

        protected static LiteralExpressionSyntax Literal(in SyntaxKind expressionKind) => SyntaxFactory.LiteralExpression(expressionKind);

        protected static LiteralExpressionSyntax NullLiteral() => Literal(SyntaxKind.NullLiteralExpression);

        protected static LiteralExpressionSyntax FalseLiteral() => Literal(SyntaxKind.FalseLiteralExpression);

        protected static LiteralExpressionSyntax TrueLiteral() => Literal(SyntaxKind.TrueLiteralExpression);

        protected static PredefinedTypeSyntax PredefinedType(in SyntaxKind kind) => SyntaxFactory.PredefinedType(kind.AsToken());

        protected static ConditionalAccessExpressionSyntax ConditionalAccess(ExpressionSyntax expression, string name) => ConditionalAccess(expression, IdentifierName(name));

        protected static ConditionalAccessExpressionSyntax ConditionalAccess(ExpressionSyntax expression, SimpleNameSyntax binding)
        {
            return SyntaxFactory.ConditionalAccessExpression(expression, SyntaxFactory.MemberBindingExpression(binding));
        }

        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax syntax, string name) => Member(syntax, IdentifierName(name));

        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax syntax, params string[] names)
        {
            var start = Member(syntax, IdentifierName(names[0]));

            var result = names.Skip(1).Aggregate(start, Member);

            return result;
        }

        protected static MemberAccessExpressionSyntax Member(ExpressionSyntax syntax, string name, params TypeSyntax[] items)
        {
            if (items is null || items.Length is 0)
            {
                return Member(syntax, IdentifierName(name));
            }

            return Member(syntax, GenericName(name, items));
        }

        protected static MemberAccessExpressionSyntax Member(string typeName, string methodName, params TypeSyntax[] items)
        {
            var type = IdentifierName(typeName);
            var method = GenericName(methodName, items);

            return Member(type, method);
        }

        protected static IdentifierNameSyntax IdentifierName(string name) => SyntaxFactory.IdentifierName(name);

        protected static IdentifierNameSyntax IdentifierName(in SyntaxToken token) => SyntaxFactory.IdentifierName(token);

        protected static GenericNameSyntax GenericName(string name, TypeSyntax[] items) => SyntaxFactory.GenericName(name).AddTypeArgumentListArguments(items);

        protected static GenericNameSyntax GenericName(string name, TypeArgumentListSyntax types) => SyntaxFactory.GenericName(name).WithTypeArgumentList(types);

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

        protected static StatementSyntax GetUpdatedStatement(StatementSyntax statement, in int spaces) => GetSyntaxWithLeadingSpaces(statement, spaces);

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