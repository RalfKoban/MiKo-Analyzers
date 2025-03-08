using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2039_ExtensionMethodsClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2039";

        public MiKo_2039_ExtensionMethodsClassSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol type && type.ContainsExtensionMethods();

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, string commentXml, IReadOnlyCollection<string> summaries)
        {
            var phrases = Constants.Comments.ExtensionMethodClassStartingPhrase;

            if (summaries.None(_ => _.StartsWithAny(phrases, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Summary, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}