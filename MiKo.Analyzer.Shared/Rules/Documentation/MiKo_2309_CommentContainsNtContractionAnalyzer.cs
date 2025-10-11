using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2309_CommentContainsNtContractionAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2309";

        public MiKo_2309_CommentContainsNtContractionAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.ContainsPhrases(Constants.Comments.NotContractionPhrase, comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia)
        {
            var locations = GetAllLocations(trivia, Constants.Comments.NotContractionPhrase, StringComparison.OrdinalIgnoreCase);

            var count = locations.Count;

            List<Diagnostic> issues = null;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    var location = locations[index];

                    var text = location.GetText(-1, 5); // Note: location.GetSurroundingWord() would report the complete word, such as 'significant'

                    if (text is "icant") // such as 'significant'
                    {
                        // this is nothing to report
                        continue;
                    }

                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name, location));
                }
            }

            return (IEnumerable<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}