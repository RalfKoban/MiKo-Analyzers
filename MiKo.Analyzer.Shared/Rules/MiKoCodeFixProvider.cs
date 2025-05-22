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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostics = context.Diagnostics;

            if (IsApplicable(diagnostics))
            {
                var diagnostic = diagnostics[0];

                var codeFix = CreateCodeFix(context, root, diagnostic);

                if (codeFix != null)
                {
                    context.RegisterCodeFix(codeFix, diagnostic);
                }
            }
        }

//// ncrunch: rdi default

        protected static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments) => SyntaxFactory.ArgumentList(arguments.ToSeparatedSyntaxList());

        protected static InvocationExpressionSyntax Invocation(MemberAccessExpressionSyntax member, params ArgumentSyntax[] arguments)
        {
            // that's for the argument
            var argumentList = ArgumentList(arguments);

            // combine both to complete call
            return SyntaxFactory.InvocationExpression(member, argumentList);
        }

        protected static InvocationExpressionSyntax Invocation(string typeName, string methodName, params TypeSyntax[] items)
        {
            // that's for the method call
            var member = SimpleMemberAccess(typeName, methodName, items);

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

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(ExpressionSyntax syntax, string name)
        {
            var identifierName = SyntaxFactory.IdentifierName(name);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, syntax, identifierName);
        }

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, string methodName, TypeSyntax[] items)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.GenericName(methodName).AddTypeArgumentListArguments(items);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static TSyntaxNode GetSyntaxWithLeadingSpaces<TSyntaxNode>(TSyntaxNode syntaxNode, in int spaces) where TSyntaxNode : SyntaxNode
        {
            var syntax = syntaxNode.WithLeadingSpaces(spaces);

            var additionalSpaces = syntax.GetPositionWithinStartLine() - syntaxNode.GetPositionWithinStartLine();

            // collect all descendant nodes that are the first ones starting on a new line, then adjust leading space for each of those
            var startingNodes = GetNodesAndTokensStartingOnSeparateLines(syntax).ToList();

            if (startingNodes.Count == 0)
            {
                return syntax;
            }

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

        protected virtual bool IsApplicable(in ImmutableArray<Diagnostic> diagnostics) => diagnostics.Any();

        protected virtual SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected virtual SyntaxToken GetToken(in SyntaxTrivia trivia, Diagnostic issue) => trivia.Token;

        protected virtual SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxToken GetUpdatedToken(in SyntaxToken token, Diagnostic issue) => token;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, in SyntaxTrivia trivia, Diagnostic issue) => null;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue) => null;

        protected virtual Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic, CancellationToken cancellationToken) => Task.FromResult(document.Project.Solution);

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var updatedSyntax = GetUpdatedSyntax(document, syntax, diagnostic);

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

            var finalRoot = GetUpdatedSyntaxRoot(document, newRoot, updatedSyntax, annotation, diagnostic) ?? newRoot;
            var newDocument = document.WithSyntaxRoot(finalRoot);

            return Task.FromResult(newDocument);
        }

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxTrivia trivia, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var oldToken = GetToken(trivia, diagnostic);
            var updatedToken = GetUpdatedToken(oldToken, diagnostic);

            var newRoot = oldToken == updatedToken ? root : root.ReplaceToken(oldToken, updatedToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(document);
            }

            var finalRoot = GetUpdatedSyntaxRoot(document, newRoot, trivia, diagnostic) ?? newRoot;
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

        private CodeAction CreateCodeFix(in CodeFixContext context, SyntaxNode root, Diagnostic issue)
        {
            var document = context.Document;

            var issueSpan = issue.Location.SourceSpan;
            var startPosition = issueSpan.Start;

            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, cancellationToken => ApplyDocumentCodeFixAsync(document, root, trivia, issue, cancellationToken), GetType().Name); // ncrunch: no coverage
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
                return CodeAction.Create(Title, cancellationToken => ApplySolutionCodeFixAsync(document, root, syntax, issue, cancellationToken), GetType().Name); // ncrunch: no coverage
            }

            return CodeAction.Create(Title, cancellationToken => ApplyDocumentCodeFixAsync(document, root, syntax, issue, cancellationToken), GetType().Name); // ncrunch: no coverage
        }

//// ncrunch: rdi default
    }
}