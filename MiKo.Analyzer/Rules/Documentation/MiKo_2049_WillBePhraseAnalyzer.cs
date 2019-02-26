using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2049_WillBePhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2049";

        private static readonly string[] WillBePhrases = CreateWillBePhrases(" will be").ToArray();

        public MiKo_2049_WillBePhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(WillBePhrases)
                                                                                                            ? new[] { ReportIssue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();

        private static IEnumerable<string> CreateWillBePhrases(params string[] forbiddenTerms) => from suffix in Constants.Comments.Delimiters from forbiddenTerm in forbiddenTerms select forbiddenTerm + suffix;
    }
}