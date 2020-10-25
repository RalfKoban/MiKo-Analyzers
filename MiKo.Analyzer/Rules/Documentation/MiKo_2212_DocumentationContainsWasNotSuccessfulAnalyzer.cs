using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2212";

        public MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment)
        {
            const string Phrase = Constants.Comments.WasNotSuccessfulPhrase;

            var index = comment.IndexOf(Phrase, StringComparison.Ordinal);
            if (index < 0)
            {
                return false;
            }

            var indexAfterPhrase = index + Phrase.Length;
            if (indexAfterPhrase == comment.Length)
            {
                // that's the last phrase
                return true;
            }

            return comment.Substring(indexAfterPhrase).StartsWithAny(Constants.Comments.Delimiters);
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => CommentHasIssue(commentXml)
                                                                                                            ? new[] { Issue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}