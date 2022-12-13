using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLocalVariableCodeFixProvider : NamingCodeFixProvider
    {
        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case VariableDeclaratorSyntax _:
                    case SingleVariableDesignationSyntax _:
                    case ForEachStatementSyntax _:
                    case ForStatementSyntax _:
                        return node;
                }
            }

            return null;
        }
    }
}