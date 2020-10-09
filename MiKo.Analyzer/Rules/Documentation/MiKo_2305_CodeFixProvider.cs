using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2305_CodeFixProvider)), Shared]
    public sealed class MiKo_2305_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer.Id;

        protected override string Title => Resources.MiKo_2305_CodeFixTitle;

        protected override bool IsTrivia => true;

        protected override SyntaxToken GetUpdatedToken(SyntaxToken token)
        {
            return token.ReplaceTrivia(token.LeadingTrivia.Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia)), ComputeReplacementTrivia);
        }

        private static SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString();

            if (MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer.CommentHasIssue(comment.TrimEnd()))
            {
                return SyntaxFactory.Comment(comment.Replace("..", "."));
            }

            return original;
        }
    }
}