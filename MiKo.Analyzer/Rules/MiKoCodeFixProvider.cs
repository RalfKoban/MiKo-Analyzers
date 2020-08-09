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

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var syntaxNodes = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf();

            var codeFix = CreateCodeFix(context.Document, syntaxNodes);
            if (codeFix != null)
            {
                context.RegisterCodeFix(codeFix, diagnostic);
            }
        }

        protected abstract Task<Solution> ApplyCodeFixAsync(Document document, SyntaxNode syntax, CancellationToken cancellationToken);

        protected abstract SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes);

        private CodeAction CreateCodeFix(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = GetSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            return CodeAction.Create(
                                     Title,
                                     _ => ApplyCodeFixAsync(document, syntax, _),
                                     GetType().Name);
        }
    }
}