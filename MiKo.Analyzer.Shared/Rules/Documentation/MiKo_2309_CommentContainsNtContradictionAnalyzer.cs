using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2309_CommentContainsNtContradictionAnalyzer : MultiLineCommentAnalyzer
    {
        public const string Id = "MiKo_2309";

        public MiKo_2309_CommentContainsNtContradictionAnalyzer() : base(Id)
        {
        }

        protected override bool CommentHasIssue(ReadOnlySpan<char> comment, SemanticModel semanticModel) => DocumentationComment.ContainsNotContradiction(comment);

        protected override IEnumerable<Diagnostic> CollectIssues(string name, SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.NotContradictionPhrase, StringComparison.OrdinalIgnoreCase).Select(_ => Issue(name, _));
    }
}