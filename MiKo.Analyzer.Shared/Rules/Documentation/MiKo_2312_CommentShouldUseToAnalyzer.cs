using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.WhichIsToTerms, StringComparison.OrdinalIgnoreCase).Select(_ => Issue(name, _, Constants.Comments.ToTerm));
    }
}