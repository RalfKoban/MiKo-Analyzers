using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
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
                var diagnostic = diagnostics.First();

                var codeFix = CreateCodeFix(context, root, diagnostic);
                if (codeFix != null)
                {
                    context.RegisterCodeFix(codeFix, diagnostic);
                }
            }
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

        protected static bool IsEnum(CodeFixContext context, ArgumentSyntax syntax)
        {
            var expression = (MemberAccessExpressionSyntax)syntax.Expression;

            if (GetSymbol(context, expression.Expression) is ITypeSymbol type)
            {
                return type.IsEnum();
            }

            return false;
        }

        protected virtual bool IsApplicable(IEnumerable<Diagnostic> diagnostics) => true;

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

            var oldToken = GetToken(trivia);
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

        protected virtual SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => null;

        protected virtual SyntaxToken GetToken(SyntaxTrivia trivia) => trivia.Token;

        protected virtual SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue) => token;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue) => null;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxTrivia trivia, Diagnostic issue) => null;

        private CodeAction CreateCodeFix(CodeFixContext context, SyntaxNode root, Diagnostic issue)
        {
            var diagnosticSpan = issue.Location.SourceSpan;
            var startPosition = diagnosticSpan.Start;

            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(context, root, trivia, issue), GetType().Name);
            }

            var syntaxNodes = root.FindToken(startPosition).Parent.AncestorsAndSelf();

            var syntax = GetSyntax(syntaxNodes.ToList());
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