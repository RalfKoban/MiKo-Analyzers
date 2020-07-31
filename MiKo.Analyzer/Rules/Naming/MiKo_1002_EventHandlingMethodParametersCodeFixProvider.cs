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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1002_EventHandlingMethodParametersCodeFixProvider)), Shared]
    public sealed class MiKo_1002_EventHandlingMethodParametersCodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1002_EventHandlingMethodParametersAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<ParameterSyntax>().First();

            // TODO: RKN maybe the equivalenceKey "Title" is wrong and should contain the name of the resulting parameter (such as "e" or "sender")
            const string Title = "Rename event argument";
            return CodeAction.Create(Title, _ => RenameAsync(document, syntax, _), Title);
        }

        private static async Task<Solution> RenameAsync(Document document, ParameterSyntax syntax, CancellationToken cancellationToken)
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