using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Rename;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FixableDiagnosticId);

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

            var codeAction = CreateCodeAction(context.Document, syntaxNodes);
            if (codeAction != null)
            {
                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        protected abstract SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes);

        protected abstract string GetNewName(ISymbol symbol);

        private CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = GetSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(document, syntax, _),
                                     GetType().Name);
        }

        private async Task<Solution> RenameSymbolAsync(Document document, SyntaxNode syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);
            var newName = GetNewName(symbol);

            // Produce a new solution that has all references to that symbol renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            // Return the new solution with the new symbol name.
            return await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, newName, originalSolution.Workspace.Options, cancellationToken)
                                .ConfigureAwait(false);
        }
    }
}