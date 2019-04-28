using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2039_ExtensionMethodsClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2039";

        public MiKo_2039_ExtensionMethodsClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.ContainsExtensionMethods();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = Constants.Comments.ExtensionMethodClassStartingPhrase;

            return summaries.All(_ => !_.StartsWithAny(phrases, StringComparison.Ordinal))
                       ? new[] { Issue(symbol, Constants.XmlTag.Summary, phrases.First()) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}