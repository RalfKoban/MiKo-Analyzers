using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

using Document = Microsoft.CodeAnalysis.Document;

namespace MiKoSolutions.Analyzers.Rules
{
    public abstract class MiKoCodeFixProvider : CodeFixProvider
    {
        protected MiKoCodeFixProvider(bool isSolutionWide) => IsSolutionWide = isSolutionWide;

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FixableDiagnosticId);

        public abstract string FixableDiagnosticId { get; }

        protected abstract string Title { get; }

        private bool IsSolutionWide { get; }

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var syntaxNodes = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf();

            var codeFix = CreateCodeFix(context.Document, root, syntaxNodes);
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
            var newDocument = updatedSyntax is null
                                  ? document
                                  : document.WithSyntaxRoot(root.ReplaceNode(syntax, updatedSyntax));
            return Task.FromResult(newDocument);
        }

        protected abstract SyntaxNode GetUpdatedSyntax(SyntaxNode syntax);

        protected abstract SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes);

        private CodeAction CreateCodeFix(Document document, SyntaxNode root, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = GetSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            return IsSolutionWide
                       ? CodeAction.Create(Title, _ => ApplySolutionCodeFixAsync(document, root, syntax, _), GetType().Name)
                       : CodeAction.Create(Title, _ => ApplyDocumentCodeFixAsync(document, root, syntax), GetType().Name);
        }
    }
}