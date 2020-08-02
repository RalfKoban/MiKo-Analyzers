using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLocalVariableCodeFixProvider : NamingCodeFixProvider
    {
        protected static SyntaxNode FindSyntax(IReadOnlyCollection<SyntaxNode> nodes)
        {
            var variableSyntax = nodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            if (variableSyntax != null)
            {
                return variableSyntax;
            }

            var variableDesignationSyntax = nodes.OfType<SingleVariableDesignationSyntax>().FirstOrDefault();
            if (variableDesignationSyntax != null)
            {
                return variableDesignationSyntax;
            }

            var forEachStatementSyntax = nodes.OfType<ForEachStatementSyntax>().FirstOrDefault();
            if (forEachStatementSyntax != null)
            {
                return forEachStatementSyntax;
            }

            return null;
        }
    }
}