using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2307";

        public MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment)
        {
            const string Phrase = Constants.Comments.WasNotSuccessfulPhrase;

            var index = comment.IndexOf(Phrase, StringComparison.Ordinal);
            if (index < 0)
            {
                return false;
            }

            var indexAfterPhrase = index + Phrase.Length;
            if (indexAfterPhrase == comment.Length)
            {
                // that's the last phrase
                return true;
            }

            return comment.Substring(indexAfterPhrase).StartsWithAny(Constants.Comments.Delimiters);
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CommentHasIssue(comment);
    }
}