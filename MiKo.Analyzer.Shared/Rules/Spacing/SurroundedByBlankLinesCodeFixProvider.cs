using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SurroundedByBlankLinesCodeFixProvider : SpacingCodeFixProvider
    {
        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
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

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            var result = syntax;

            if (issue.Properties.ContainsKey(Constants.AnalyzerCodeFixSharedData.NoLineBefore))
            {
                result = result.WithLeadingEmptyLine();
            }

            if (issue.Properties.ContainsKey(Constants.AnalyzerCodeFixSharedData.NoLineAfter))
            {
                result = result.HasTrailingComment()
                         ? result.WithAdditionalTrailingEmptyLine()
                         : result.WithTrailingEmptyLine();
            }

            return result;
        }
    }
}