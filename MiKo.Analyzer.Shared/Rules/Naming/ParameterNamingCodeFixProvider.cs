using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class ParameterNamingCodeFixProvider : NamingCodeFixProvider
    {
        protected virtual string FindBetterName(IParameterSymbol symbol) => null;

        protected virtual string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => FindBetterName(symbol);

        protected sealed override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => FindBetterName((IParameterSymbol)symbol, diagnostic);

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().First();
    }
}