﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> CollectIssues(string name, in SyntaxTrivia trivia) => GetAllLocations(trivia, Constants.Comments.NotContractionPhrase, StringComparison.OrdinalIgnoreCase).Select(_ => Issue(name, _));
    }
}