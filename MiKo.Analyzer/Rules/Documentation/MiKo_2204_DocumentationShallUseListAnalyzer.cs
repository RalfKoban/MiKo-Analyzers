using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2204_DocumentationShallUseListAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2204";

        private static readonly string[] Delimiters = { ".)", ".", ")", ":" };

        private static readonly string[] Triggers = Enumerable.Concat(
                                                      new[] { "-", "--", "---" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => delimiter + " " + _ + " "),
                                                      new[] { "1", "a", "2", "b", "3", "c" }.SelectMany(_ => Delimiters, (_, delimiter) => " " + _ + delimiter + " ")
                                                      ).ToArray();

        public MiKo_2204_DocumentationShallUseListAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(Triggers, StringComparison.OrdinalIgnoreCase)
                                                                                                            ? new[] { Issue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}