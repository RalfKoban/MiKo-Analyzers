using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SingleLineCommentCodeFixProvider : DocumentationCodeFixProvider
    {
        protected sealed override SyntaxToken GetUpdatedToken(SyntaxToken token)
        {
            return token.ReplaceTrivia(token.LeadingTrivia.Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia)), ComputeReplacementTrivia);
        }

        protected abstract SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten);
    }
}