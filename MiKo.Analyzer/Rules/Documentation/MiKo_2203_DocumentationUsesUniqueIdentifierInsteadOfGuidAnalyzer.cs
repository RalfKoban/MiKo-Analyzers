using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2203";

        internal static readonly string[] Guids = new[] { "guid", " Guid", "GUID" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => " " + _ + delimiter).ToArray();

        public MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml) => commentXml.ContainsAny(Guids, StringComparison.Ordinal)
                                                                                                                                     ? new[] { Issue(symbol) }
                                                                                                                                     : Enumerable.Empty<Diagnostic>();
    }
}