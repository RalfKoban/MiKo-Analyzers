using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

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

            var diagnostic = context.Diagnostics.First();
            var codeFix = CreateCodeFix(context.Document, root, diagnostic);
            if (codeFix != null)
            {
                context.RegisterCodeFix(codeFix, diagnostic);
            }
        }

        protected static ISymbol GetSymbol(Document document, SyntaxNode syntax) => GetSymbolAsync(document, syntax, CancellationToken.None).Result;

        protected static async Task<ISymbol> GetSymbolAsync(Document document, SyntaxNode syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            return semanticModel.GetDeclaredSymbol(syntax, cancellationToken);
        }

        protected virtual Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic, CancellationToken cancellationToken) => Task.FromResult(document.Project.Solution);

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var updatedSyntax = GetUpdatedSyntax(document, syntax, diagnostic);
            if (ReferenceEquals(updatedSyntax, syntax))
            {
                return Task.FromResult(document);
            }

            var newRoot = updatedSyntax is null
                              ? root.RemoveNode(syntax, SyntaxRemoveOptions.KeepNoTrivia)
                              : root.ReplaceNode(syntax, updatedSyntax);

            if (newRoot is null)
            {
                return Task.FromResult(document);
            }

            var finalRoot = GetUpdatedSyntaxRoot(newRoot) ?? newRoot;
            var newDocument = document.WithSyntaxRoot(finalRoot);

            return Task.FromResult(newDocument);
        }

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxTrivia trivia, Diagnostic diagnostic)
        {
            var oldToken = GetToken(trivia);
            var updatedToken = GetUpdatedToken(oldToken, diagnostic);

            var newDocument = document.WithSyntaxRoot(root.ReplaceToken(oldToken, updatedToken));

            return Task.FromResult(newDocument);
        }

        protected virtual SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => null;

        protected virtual SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => null;

        protected virtual SyntaxToken GetToken(SyntaxTrivia trivia) => trivia.Token;

        protected virtual SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root) => null;

        protected virtual SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic diagnostic) => token;

        private CodeAction CreateCodeFix(Document document, SyntaxNode root, Diagnostic diagnostic)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var startPosition = diagnosticSpan.Start;

            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(document, root, trivia, diagnostic), GetType().Name);
            }

            var syntaxNodes = root.FindToken(startPosition).Parent.AncestorsAndSelf();

            var syntax = GetSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            if (IsSolutionWide)
            {
                return CodeAction.Create(Title, _ => ApplySolutionCodeFixAsync(document, root, syntax, diagnostic, _), GetType().Name);
            }

            return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(document, root, syntax, diagnostic), GetType().Name);
        }
    }
}