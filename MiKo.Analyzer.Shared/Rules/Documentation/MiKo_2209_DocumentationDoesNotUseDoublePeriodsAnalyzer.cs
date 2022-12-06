using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2209";

        private static readonly HashSet<char> AllowedChars = new HashSet<char>
                                                                 {
                                                                     '.',
                                                                     '/',
                                                                     '\\',
                                                                 };

        public MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(ReadOnlySpan<char> comment) => comment.Contains("..", _ => AllowedChars.Contains(_) is false, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            if (CommentHasIssue(commentXml.AsSpan()))
            {
                yield return Issue(symbol);
            }
        }
    }
}