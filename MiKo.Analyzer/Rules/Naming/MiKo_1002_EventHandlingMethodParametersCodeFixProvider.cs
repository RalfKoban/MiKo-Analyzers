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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1002_EventHandlingMethodParametersCodeFixProvider)), Shared]
    public sealed class MiKo_1002_EventHandlingMethodParametersCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Rename event argument";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MiKo_1002_EventHandlingMethodParametersAnalyzer.Id);

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var syntax = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ParameterSyntax>().First();

            // TODO: RKN maybe the equivalenceKey "Title" is wrong and should contain the name of the resulting parameter (such as "e" or "sender")
            context.RegisterCodeFix(CodeAction.Create(Title, _ => RenameAsync(context.Document, syntax, _), Title), diagnostic);
        }

        private async Task<Solution> RenameAsync(Document document, ParameterSyntax syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);

            var newName = symbol.Type.IsObject() ? "sender" : "e";

            // Produce a new solution that has all references to that exception renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            // Return the new solution with the renamed code
            return await Renamer.RenameSymbolAsync(originalSolution, symbol, newName, originalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);
        }
    }
}