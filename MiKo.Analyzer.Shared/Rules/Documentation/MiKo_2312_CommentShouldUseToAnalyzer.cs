using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2312_CommentShouldUseToAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2312";

        public MiKo_2312_CommentShouldUseToAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(in ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.ContainsPhrases(Constants.Comments.WhichIsToTerms, comment);

        protected override IReadOnlyList<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => trivia.GetAllLocations(Constants.Comments.WhichIsToTerms, StringComparison.OrdinalIgnoreCase)
                                                                                                                 .ToArray(_ => Issue(name, _, Constants.Comments.ToTerm));
    }
}