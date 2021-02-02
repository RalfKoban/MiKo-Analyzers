using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class SurroundedByBlankLinesCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<ExpressionStatementSyntax>().First();
        }

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var result = syntax;

            if (diagnostic.Properties.ContainsKey(SurroundedByBlankLinesAnalyzer.NoLineBefore))
            {
                result = result.WithLeadingEmptyLine();
            }

            if (diagnostic.Properties.ContainsKey(SurroundedByBlankLinesAnalyzer.NoLineAfter))
            {
                result = result.WithTrailingEmptyLine();
            }

            return result;
        }
    }
}