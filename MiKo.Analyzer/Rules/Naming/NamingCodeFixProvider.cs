﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Rename;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingCodeFixProvider : MiKoCodeFixProvider
    {
        protected NamingCodeFixProvider() : base(true)
        {
        }

        protected sealed override async Task<Solution> ApplySolutionCodeFixAsync(Document document, SyntaxNode root, SyntaxNode syntax, CancellationToken cancellationToken)
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

        protected abstract string GetNewName(ISymbol symbol);

        protected sealed override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => throw new NotSupportedException("This code fix provider does not modify the syntax");
    }
}