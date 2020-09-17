using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2303_CodeFixProvider)), Shared]
    public sealed class MiKo_2303_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer.Id;

        protected override string Title => Resources.MiKo_2303_CodeFixTitle;

        protected override bool IsTrivia => true;

        protected override SyntaxToken GetUpdatedToken(SyntaxToken token)
        {
            return token.ReplaceTrivia(token.LeadingTrivia.Where(_ => _.IsKind(SyntaxKind.SingleLineCommentTrivia)), ComputeReplacementTrivia);
        }

        private static SyntaxTrivia ComputeReplacementTrivia(SyntaxTrivia original, SyntaxTrivia rewritten)
        {
            var comment = original.ToString().TrimEnd();

            if (MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer.CommentHasIssue(comment))
            {
                // ensure that there is no empty space at the end, after removing trailing dots
                return SyntaxFactory.Comment(comment.TrimEnd('.').TrimEnd());
            }

            return original;
        }
    }
}