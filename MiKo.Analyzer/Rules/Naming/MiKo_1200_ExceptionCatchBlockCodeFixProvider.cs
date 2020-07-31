using System.Collections.Generic;
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
    public sealed class MiKo_1200_ExceptionCatchBlockCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Rename exception";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MiKo_1200_ExceptionCatchBlockAnalyzer.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
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

        private static CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<CatchDeclarationSyntax>().First();

            // TODO: RKN maybe the equivalenceKey "Title" is wrong and should contain the name of the resulting parameter (such as "e" or "sender")
            return CodeAction.Create(Title, _ => RenameAsync(document, syntax, _), Title);
        }

        private static async Task<Solution> RenameAsync(Document document, CatchDeclarationSyntax syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);

            // Produce a new solution that has all references to that exception renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            // Return the new solution with the renamed code
            return await Renamer.RenameSymbolAsync(originalSolution, symbol, "ex", originalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);
        }
    }
}
