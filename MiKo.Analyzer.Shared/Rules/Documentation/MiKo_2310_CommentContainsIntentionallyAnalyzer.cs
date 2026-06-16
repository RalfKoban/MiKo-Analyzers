using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2310_CommentContainsIntentionallyAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2310";

        public MiKo_2310_CommentContainsIntentionallyAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeCommentTrivia(string name, SyntaxTrivia[] triviaToAnalyze, SemanticModel semanticModel)
        {
            List<Diagnostic> issues = null;

            foreach (var trivia in triviaToAnalyze.GroupBy(_ => _.Token))
            {
                var sb = StringBuilderCache.Acquire();

                foreach (var t in trivia)
                {
                    sb.Append(t.ToString());
                }

                var comment = sb.ToStringAndRelease();

                if (CommentHasIssue(comment))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    foreach (var t in trivia)
                    {
                        var locations = t.GetAllLocations(Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase);

                        for (int index = 0, locationsCount = locations.Count; index < locationsCount; index++)
                        {
                            issues.Add(Issue(name, locations[index]));
                        }
                    }
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
        }

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => false; // we've overridden AnalyzeCommentTrivia, so this is not used

        private static bool CommentHasIssue(string comment)
        {
            if (comment.ContainsAny(Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return comment.ContainsAny(Constants.Comments.ReasoningPhrases, StringComparison.OrdinalIgnoreCase) is false;
            }

            return false;
        }
    }
}