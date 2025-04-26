using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class CommentCodeFixProvider : DocumentationCodeFixProvider
    {
        protected override bool IsTrivia => true;

        protected sealed override SyntaxToken GetUpdatedToken(in SyntaxToken token, Diagnostic issue)
        {
            return token.ReplaceTrivia(token.GetComment(), (original, rewritten) => ComputeReplacementTrivia(original, rewritten));
        }

        protected abstract SyntaxTrivia ComputeReplacementTrivia(in SyntaxTrivia original, in SyntaxTrivia rewritten);
    }
}