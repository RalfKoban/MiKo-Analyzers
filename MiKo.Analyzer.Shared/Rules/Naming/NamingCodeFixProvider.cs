using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingCodeFixProvider : MiKoCodeFixProvider
    {
        protected static readonly SyntaxKind[] MethodKinds =
                                                             {
                                                                 SyntaxKind.LocalFunctionStatement,
                                                                 SyntaxKind.MethodDeclaration,
                                                             };

        protected override bool IsSolutionWide => true;

        protected sealed override async Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            // Produce a new solution that has all references to that symbol renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            var symbol = await GetSymbolAsync(document, syntax, cancellationToken).ConfigureAwait(false);

            var newName = GetNewName(diagnostic, symbol);

            if (newName.IsNullOrWhiteSpace() || newName.Equals(symbol.Name, StringComparison.Ordinal))
            {
                // nothing changed
                return originalSolution;
            }

#pragma warning disable CS0618 // Type or member is obsolete : Required as we still are running in 2019 and there the overload is not available

            // Return the new solution with the new symbol name.
            return await Renamer.RenameSymbolAsync(originalSolution, symbol, newName, originalSolution.Workspace.Options, cancellationToken).ConfigureAwait(false);

#pragma warning restore CS0618 // Type or member is obsolete
        }

        protected string GetNewName(Diagnostic diagnostic, ISymbol symbol) => diagnostic.Properties.TryGetValue(Constants.BetterName, out var betterName) ? betterName : symbol.Name;

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => throw new NotSupportedException("This code fix provider does not modify the syntax");
    }
}