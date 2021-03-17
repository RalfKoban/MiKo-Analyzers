using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class SurroundedByBlankLinesCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case ExpressionStatementSyntax _:
                    case IfStatementSyntax _:
                        return node;
                }
            }

            return null;
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