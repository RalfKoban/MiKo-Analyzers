﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Rename;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingCodeFixProvider : MiKoCodeFixProvider
    {
        protected override bool IsSolutionWide => true;

        protected sealed override async Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            // Produce a new solution that has all references to that symbol renamed, including the declaration.
            var originalSolution = document.Project.Solution;

            var symbol = await GetSymbolAsync(document, syntax, cancellationToken);
            var newName = GetNewName(diagnostic, symbol);

            if (newName.IsNullOrWhiteSpace() || string.Equals(symbol.Name, newName))
            {
                // nothing changed
                return originalSolution;
            }

            // Return the new solution with the new symbol name.
            return await Renamer.RenameSymbolAsync(originalSolution, symbol, newName, originalSolution.Workspace.Options, cancellationToken)
                                .ConfigureAwait(false);
        }

        protected abstract string GetNewName(Diagnostic diagnostic, ISymbol symbol);

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => throw new NotSupportedException("This code fix provider does not modify the syntax");
    }
}