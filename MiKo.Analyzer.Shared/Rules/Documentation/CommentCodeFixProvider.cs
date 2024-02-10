using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class CommentCodeFixProvider : DocumentationCodeFixProvider
    {
        protected override bool IsTrivia => true;

        protected sealed override SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue)
        {
            return token.ReplaceTrivia(token.GetComment(), ComputeReplacementTrivia);
        }

        protected abstract SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten);
    }
}