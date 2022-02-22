using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class ParameterNamingCodeFixProvider : NamingCodeFixProvider
    {
        protected abstract string FindBetterName(IParameterSymbol symbol);

        protected sealed override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => FindBetterName((IParameterSymbol)symbol);

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().First();
    }
}