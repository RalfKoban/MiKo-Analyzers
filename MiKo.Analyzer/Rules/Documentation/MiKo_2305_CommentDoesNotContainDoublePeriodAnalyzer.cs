using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2305";

        public MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(string comment, SemanticModel semanticModel) => CommentHasIssue(comment);

        private static bool CommentHasIssue(string comment)
        {
            const char Period = '.';
            const string Value = "..";

            var index = 0;
            var valueLength = Value.Length;
            var commentLength = comment.Length;

            while (true)
            {
                index = comment.IndexOf(Value, index, StringComparison.OrdinalIgnoreCase);

                if (index <= -1)
                    break;

                var positionAfterCharacter = index + valueLength;
                if (positionAfterCharacter >= commentLength || comment[positionAfterCharacter] != Period)
                {
                    return true;
                }

                index = positionAfterCharacter;
            }

            return false;
        }
    }
}