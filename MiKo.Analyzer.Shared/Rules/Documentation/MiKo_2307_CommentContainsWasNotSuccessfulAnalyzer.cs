using System;
using System.Collections.Generic;

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

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.ContainsPhrase(Constants.Comments.WasNotSuccessfulPhrase, comment);

        protected override IReadOnlyList<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => trivia.GetAllLocations(Constants.Comments.WasNotSuccessfulPhrase)
                                                                                                                 .ToArray(_ => Issue(name, _));
    }
}