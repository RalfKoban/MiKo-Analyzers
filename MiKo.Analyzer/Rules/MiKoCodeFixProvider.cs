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
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var codeFix = CreateCodeFix(context.Document, root, diagnosticSpan.Start);
            if (codeFix != null)
            {
                context.RegisterCodeFix(codeFix, diagnostic);
            }
        }

        protected virtual Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, CancellationToken cancellationToken)
            => Task.FromResult(document.Project.Solution);

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);
            if (updatedSyntax is null || ReferenceEquals(updatedSyntax, syntax))
            {
                return Task.FromResult(document);
            }

            var newDocument = document.WithSyntaxRoot(root.ReplaceNode(syntax, updatedSyntax));
            return Task.FromResult(newDocument);
        }

        protected Task<Document> ApplyDocumentCodeFixAsync(Document document, SyntaxNode root, SyntaxTrivia trivia)
        {
            var oldToken = GetToken(trivia);
            var updatedToken = GetUpdatedToken(oldToken);
            var newDocument = document.WithSyntaxRoot(root.ReplaceToken(oldToken, updatedToken));
            return Task.FromResult(newDocument);
        }

        protected virtual SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => null;

        protected virtual SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => null;

        protected virtual SyntaxToken GetToken(SyntaxTrivia trivia) => trivia.Token;

        protected virtual SyntaxToken GetUpdatedToken(SyntaxToken token) => token;

        private CodeAction CreateCodeFix(Document document, SyntaxNode root, int startPosition)
        {
            if (IsTrivia)
            {
                var trivia = root.FindTrivia(startPosition);

                return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(document, root, trivia), GetType().Name);
            }

            var syntaxNodes = root.FindToken(startPosition).Parent.AncestorsAndSelf();

            var syntax = GetSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            if (IsSolutionWide)
            {
                return CodeAction.Create(Title, _ => ApplySolutionCodeFixAsync(document, root, syntax, _), GetType().Name);
            }

            return CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(document, root, syntax), GetType().Name);
        }
    }
}