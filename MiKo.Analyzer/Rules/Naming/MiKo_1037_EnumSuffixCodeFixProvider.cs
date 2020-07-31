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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1037_EnumSuffixCodeFixProvider)), Shared]
    public sealed class MiKo_1037_EnumSuffixCodeFixProvider : NamingCodeFixProvider
    {
        private const string Title = "Remove 'Enum' suffix";

        public override string FixableDiagnosticId => MiKo_1037_EnumSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntaxes = syntaxNodes.ToList();

            var enumSyntax = syntaxes.OfType<EnumDeclarationSyntax>().FirstOrDefault();
            if (enumSyntax != null)
            {
                return CodeAction.Create(Title, c => RenameEnumAsync(document, enumSyntax, c), Title);
            }

            var typeSyntax = syntaxes.OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeSyntax != null)
            {
                return CodeAction.Create(Title, c => RenameTypeAsync(document, typeSyntax, c), Title);
            }

            return null;
        }

        private static async Task<Solution> RenameEnumAsync(Document document, EnumDeclarationSyntax syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);

            return await RenameSymbolAsync(document, cancellationToken, symbol);
        }

        private static async Task<Solution> RenameTypeAsync(Document document, BaseTypeDeclarationSyntax syntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);

            return await RenameSymbolAsync(document, cancellationToken, symbol);
        }

        private static async Task<Solution> RenameSymbolAsync(Document document, CancellationToken cancellationToken, ITypeSymbol symbol)
        {
            var newName = MiKo_1037_EnumSuffixAnalyzer.FindBetterName(symbol);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            // Return the new solution with the new type name.
            return await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, newName, originalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);
        }
    }
}
