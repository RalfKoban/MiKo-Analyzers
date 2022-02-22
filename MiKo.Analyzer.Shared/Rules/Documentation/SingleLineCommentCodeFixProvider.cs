using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SingleLineCommentCodeFixProvider : DocumentationCodeFixProvider
    {
        protected override bool IsTrivia => true;

        protected sealed override SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue)
        {
            return token.ReplaceTrivia(token.GetAllTrivia().Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia)), ComputeReplacementTrivia);
        }

        protected abstract SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten);
    }
}