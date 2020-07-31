using System.Collections.Generic;
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
    public sealed class MiKo_1200_ExceptionCatchBlockCodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1200_ExceptionCatchBlockAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<CatchDeclarationSyntax>().First();

            const string Title = "Rename exception";
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
