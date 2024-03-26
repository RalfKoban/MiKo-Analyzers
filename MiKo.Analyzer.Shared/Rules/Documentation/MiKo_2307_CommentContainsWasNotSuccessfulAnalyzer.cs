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

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.ContainsPhrase(Constants.Comments.WasNotSuccessfulPhrase, comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.WasNotSuccessfulPhrase).Select(_ => Issue(name, _));
    }
}