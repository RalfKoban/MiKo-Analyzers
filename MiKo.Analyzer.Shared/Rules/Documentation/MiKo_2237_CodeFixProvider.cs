using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2237_CodeFixProvider)), Shared]
    public sealed class MiKo_2237_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2237";

        protected override bool IsTrivia => true;

        protected override SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue)
        {
            var issueLocation = issue.Location;

            var leadingTrivia = token.LeadingTrivia;
            var leadingTriviaCount = leadingTrivia.Count;

            for (var index = 1; index < leadingTriviaCount; index++)
            {
                var trivia = leadingTrivia[index];

                if (trivia.GetLocation().Equals(issueLocation))
                {
                    return token.WithLeadingTrivia(leadingTrivia.RemoveAt(index - 1));
                }
            }

            return token;
        }
    }
}