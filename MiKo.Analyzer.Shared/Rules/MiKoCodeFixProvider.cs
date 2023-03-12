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

        protected static MemberAccessExpressionSyntax SimpleMemberAccess(string typeName, string methodName, TypeSyntax[] items)
        {
            var type = SyntaxFactory.IdentifierName(typeName);
            var method = SyntaxFactory.GenericName(methodName).AddTypeArgumentListArguments(items);

            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
        }

        protected static SemanticModel GetSemanticModel(CodeFixContext context) => context.Document.GetSemanticModelAsync(context.CancellationToken).Result;

        protected static ISymbol GetSymbol(CodeFixContext context, SyntaxNode syntax) => GetSymbolAsync(context, syntax, CancellationToken.None).Result;

        protected static async Task<ISymbol> GetSymbolAsync(CodeFixContext context, SyntaxNode syntax, CancellationToken cancellationToken)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);

            if (syntax is TypeSyntax typeSyntax)
            {
               return semanticModel?.GetTypeInfo(typeSyntax, cancellationToken).Type;
            }

            return semanticModel?.GetDeclaredSymbol(syntax, cancellationToken);
        }

        protected static bool IsConst(CodeFixContext context, ArgumentSyntax syntax)
        {
            var identifierName = syntax.Expression.GetName();

            var method = syntax.GetEnclosingMethod(GetSemanticModel(context));
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

        protected static bool IsEnum(CodeFixContext context, ArgumentSyntax syntax)
        {
            var expression = (MemberAccessExpressionSyntax)syntax.Expression;

            if (GetSymbol(context, expression.Expression) is ITypeSymbol type)
            {
                return type.IsEnum();
            }

            return false;
        }

        protected virtual bool IsApplicable(IEnumerable<Diagnostic> diagnostics) => diagnostics.Any();

        protected virtual Task<Solution> ApplySolutionCodeFixAsync(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic, CancellationToken cancellationToken) => Task.FromResult(context.Document.Project.Solution);

        protected Task<Document> ApplyDocumentCodeFixAsync(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(context.Document);
            }

            var updatedSyntax = GetUpdatedSyntax(context, syntax, diagnostic);

            var newRoot = root;

            if (ReferenceEquals(updatedSyntax, syntax) is false)
            {
                newRoot = updatedSyntax is null
                              ? root.Without(syntax)
                              : root.ReplaceNode(syntax, updatedSyntax);

                if (newRoot is null)
                {
                    return Task.FromResult(context.Document);
                }
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(context.Document);
            }

            var finalRoot = GetUpdatedSyntaxRoot(context, newRoot, updatedSyntax, diagnostic) ?? newRoot;
            var newDocument = context.Document.WithSyntaxRoot(finalRoot);

            return Task.FromResult(newDocument);
        }

        protected Task<Document> ApplyDocumentCodeFixAsync(CodeFixContext context, SyntaxNode root, SyntaxTrivia trivia, Diagnostic diagnostic)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(context.Document);
            }

            var oldToken = GetToken(trivia, diagnostic);
            var updatedToken = GetUpdatedToken(oldToken, diagnostic);

            var newRoot = oldToken == updatedToken ? root : root.ReplaceToken(oldToken, updatedToken);

            if (context.CancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(context.Document);
            }

            var finalRoot = GetUpdatedSyntaxRoot(context, newRoot, trivia, diagnostic) ?? newRoot;
            var newDocument = context.Document.WithSyntaxRoot(finalRoot);

            return Task.FromResult(newDocument);
        }

        protected virtual SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => null;

        protected virtual SyntaxToken GetToken(SyntaxTrivia trivia, Diagnostic issue) => trivia.Token;

        protected virtual SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue) => token;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxTrivia trivia, Diagnostic issue) => null;

        private CodeAction CreateCodeFix(CodeFixContext context, SyntaxNode root, Diagnostic issue)
        {
            var issueSpan = issue.Location.SourceSpan;
            var startPosition = issueSpan.Start;

            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(context, root, trivia, issue), GetType().Name);
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
                return CodeAction.Create(Title, _ => ApplySolutionCodeFixAsync(context, root, syntax, issue, _), GetType().Name);
            }

            return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(context, root, syntax, issue), GetType().Name);
        }
    }
}