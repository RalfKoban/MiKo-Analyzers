using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
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

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var result = syntax;

            if (issue.Properties.ContainsKey(SurroundedByBlankLinesAnalyzer.NoLineBefore))
            {
                result = result.WithLeadingEmptyLine();
            }

            if (issue.Properties.ContainsKey(SurroundedByBlankLinesAnalyzer.NoLineAfter))
            {
                result = result.WithTrailingEmptyLine();
            }

            return result;
        }
    }
}