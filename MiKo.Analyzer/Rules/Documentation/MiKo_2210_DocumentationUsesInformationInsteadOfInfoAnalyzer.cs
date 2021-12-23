using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2210";

        internal const string Term = "info";

        internal static readonly string[] Terms = Constants.Comments.Delimiters.Select(_ => " " + Term + _).ToArray();

        public MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml) => commentXml.ContainsAny(Terms)
                                                                                                                                     ? new[] { Issue(symbol) }
                                                                                                                                     : Enumerable.Empty<Diagnostic>();
    }
}