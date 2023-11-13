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
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FixableDiagnosticId);

        public abstract string FixableDiagnosticId { get; }

        protected abstract string Title { get; }

        protected virtual bool IsSolutionWide => false;

        protected virtual bool IsTrivia => false;

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

//// ncrunch: collect values off
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
//// ncrunch: collect values default

        protected static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments) => SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));

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

        protected static IsPatternExpressionSyntax IsPattern(ExpressionSyntax operand, LiteralExpressionSyntax literal) => SyntaxFactory.IsPatternExpression(operand, SyntaxFactory.ConstantPattern(literal));

        protected static IsPatternExpressionSyntax IsFalsePattern(ExpressionSyntax operand) => IsPattern(operand, Literal(SyntaxKind.FalseLiteralExpression));

        protected static IsPatternExpressionSyntax IsTruePattern(ExpressionSyntax operand) => IsPattern(operand, Literal(SyntaxKind.TrueLiteralExpression));

        protected static IsPatternExpressionSyntax IsNullPattern(ExpressionSyntax operand) => IsPattern(operand, Literal(SyntaxKind.NullLiteralExpression));

        protected static LiteralExpressionSyntax Literal(SyntaxKind expressionKind) => SyntaxFactory.LiteralExpression(expressionKind);

        protected static PredefinedTypeSyntax PredefinedType(SyntaxKind kind) => SyntaxFactory.PredefinedType(Token(kind));

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

        protected static SyntaxToken Token(SyntaxKind kind) => SyntaxFactory.Token(kind);

        protected static SyntaxTokenList TokenList(params SyntaxKind[] syntaxKinds) => TokenList((IEnumerable<SyntaxKind>)syntaxKinds);

        protected static SyntaxTokenList TokenList(IEnumerable<SyntaxKind> syntaxKinds) => SyntaxFactory.TokenList(syntaxKinds.Select(Token));

        protected static SemanticModel GetSemanticModel(Document document) => document.GetSemanticModelAsync(CancellationToken.None).Result;

        protected static ISymbol GetSymbol(Document document, SyntaxNode syntax) => GetSymbolAsync(document, syntax, CancellationToken.None).Result;

        protected static async Task<ISymbol> GetSymbolAsync(Document document, SyntaxNode syntax, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (syntax is TypeSyntax typeSyntax)
            {
                return semanticModel?.GetTypeInfo(typeSyntax, cancellationToken).Type;
            }

            return semanticModel?.GetDeclaredSymbol(syntax, cancellationToken);
        }

        protected static bool IsConst(Document document, ArgumentSyntax syntax)
        {
            var identifierName = syntax.Expression.GetName();

            var method = syntax.GetEnclosingMethod(GetSemanticModel(document));
            var type = method.FindContainingType();

            var isConst = type.GetMembers(identifierName).OfType<IFieldSymbol>().Any(_ => _.IsConst);

            if (isConst)
            {
                // const value inside class
                return true;
            }

            // local const variable
            var isLocalConst = method.GetSyntax().DescendantNodes<LocalDeclarationStatementSyntax>(_ => _.IsConst)
                                     .Any(_ => _.Declaration.Variables.Any(__ => __.GetName() == identifierName));

            return isLocalConst;
        }

        protected static bool IsEnum(Document document, ArgumentSyntax syntax)
        {
            var expression = (MemberAccessExpressionSyntax)syntax.Expression;

            if (GetSymbol(document, expression.Expression) is ITypeSymbol type)
            {
                return type.IsEnum();
            }

            return false;
        }

        protected virtual bool IsApplicable(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Any();

        protected virtual SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => null;

        protected virtual SyntaxToken GetToken(SyntaxTrivia trivia, Diagnostic issue) => trivia.Token;

        protected virtual SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue) => token;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxTrivia trivia, Diagnostic issue) => null;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue) => null;

//// ncrunch: collect values off
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

        private CodeAction CreateCodeFix(CodeFixContext context, SyntaxNode root, Diagnostic issue)
        {
            var document = context.Document;

            var issueSpan = issue.Location.SourceSpan;
            var startPosition = issueSpan.Start;

            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, token => ApplyDocumentCodeFixAsync(document, root, trivia, issue, token), GetType().Name);
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
                return CodeAction.Create(Title, token => ApplySolutionCodeFixAsync(document, root, syntax, issue, token), GetType().Name);
            }

            return CodeAction.Create(Title, token => ApplyDocumentCodeFixAsync(document, root, syntax, issue, token), GetType().Name);
        }
//// ncrunch: collect values default
    }
}