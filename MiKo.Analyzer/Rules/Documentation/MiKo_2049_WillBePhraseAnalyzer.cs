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

        private static readonly string[] Phrases = Constants.Comments.Delimiters.SelectMany(_ => new[] { " will be", " will also be", " will as well be" }, (delimiter, phrase) => phrase + delimiter).ToArray();

        public MiKo_2049_WillBePhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(Phrases)
                                                                                                            ? new[] { Issue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}