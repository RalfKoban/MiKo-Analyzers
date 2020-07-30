using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1200_ExceptionCatchBlockCodeFixProvider)), Shared]
    public class MiKo_1200_ExceptionCatchBlockCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Rename exception";
        private const string NewName = "ex";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MiKo_1200_ExceptionCatchBlockAnalyzer.Id);

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the catch declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<CatchDeclarationSyntax>().First();

            context.RegisterCodeFix(CodeAction.Create(Title, _ => RenameAsync(context.Document, declaration, _), Title), diagnostic);
        }

        private async Task<Solution> RenameAsync(Document document, CatchDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var symbol = semanticModel.GetDeclaredSymbol(declaration, cancellationToken);

            // Produce a new solution that has all references to that exception renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            // Return the new solution with the renamed code
            return await Renamer.RenameSymbolAsync(originalSolution, symbol, NewName, originalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);
        }
    }
}
