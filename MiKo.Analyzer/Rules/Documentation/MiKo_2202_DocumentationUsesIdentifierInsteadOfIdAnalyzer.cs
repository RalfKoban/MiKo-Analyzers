using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2202";

        internal const string Term = "id";

        internal static readonly string[] Terms = Constants.Comments.Delimiters.Select(_ => " " + Term + _).ToArray();

        public MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(Terms)
                                                                                                            ? new[] { Issue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}