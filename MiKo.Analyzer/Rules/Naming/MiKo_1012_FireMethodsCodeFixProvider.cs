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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1012_FireMethodsCodeFixProvider)), Shared]
    public sealed class MiKo_1012_FireMethodsCodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1037_EnumSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<MethodDeclarationSyntax>().First();

            const string Title = "Rename 'fire' to 'raise'";
            return CodeAction.Create(Title, c => RenameSymbolAsync(document, syntax, c), Title);
        }

        private static async Task<Solution> RenameSymbolAsync(Document document, BaseMethodDeclarationSyntax syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);

            var newName = MiKo_1012_FireMethodsAnalyzer.FindBetterName(symbol);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            // Return the new solution with the new type name.
            return await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, newName, originalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);
        }
    }
}