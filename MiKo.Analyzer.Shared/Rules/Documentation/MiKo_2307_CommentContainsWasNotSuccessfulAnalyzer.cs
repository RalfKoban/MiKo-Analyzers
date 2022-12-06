using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static bool CommentHasIssue(ReadOnlySpan<char> comment)
        {
            const string Phrase = Constants.Comments.WasNotSuccessfulPhrase;

            var index = comment.IndexOf(Phrase.AsSpan(), StringComparison.Ordinal);
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

            return comment.Slice(indexAfterPhrase).StartsWithAny(Constants.Comments.Delimiters);
        }

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel) => CommentHasIssue(comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.WasNotSuccessfulPhrase).Select(_ => Issue(name, _));
    }
}