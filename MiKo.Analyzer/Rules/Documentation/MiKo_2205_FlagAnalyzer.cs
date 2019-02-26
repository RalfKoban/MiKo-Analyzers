using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2205_FlagAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2205";

        private static readonly string[] FlagPhrases = CreatePhrases(" flag", " flags").ToArray();

        public MiKo_2205_FlagAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(FlagPhrases)
                                                                                                            ? new[] { ReportIssue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();

        private static IEnumerable<string> CreatePhrases(params string[] forbiddenTerms) => from suffix in Constants.Comments.Delimiters from forbiddenTerm in forbiddenTerms select forbiddenTerm + suffix;
    }
}