using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2214_DocumentationContainsEmptyLinesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2214";

        private static readonly char[] LineEndings = { '\n' };

        public MiKo_2214_DocumentationContainsEmptyLinesAnalyzer() : base(Id)
        {
        }

        internal static bool CommentHasIssue(string comment)
        {
            if (comment.IsNullOrWhiteSpace())
            {
                return false;
            }

            var allLines = comment.Split(LineEndings, StringSplitOptions.None);

            return allLines.Any(_ => _.IsNullOrWhiteSpace());
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml) => CommentHasIssue(commentXml?.Trim())
                                                                                                                                     ? new[] { Issue(symbol) }
                                                                                                                                     : Enumerable.Empty<Diagnostic>();
    }
}