using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLocalVariableCodeFixProvider : NamingCodeFixProvider
    {
        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> nodes)
        {
            foreach (var node in nodes)
            {
                switch (node)
                {
                    case VariableDeclaratorSyntax vds: return vds;
                    case SingleVariableDesignationSyntax svds: return svds;
                    case ForEachStatementSyntax fess: return fess;
                    case ForStatementSyntax fss: return fss;
                }
            }

            return null;
        }
    }
}