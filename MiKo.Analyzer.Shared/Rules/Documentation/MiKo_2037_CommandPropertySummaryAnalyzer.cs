using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2037_CommandPropertySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2037";

        public MiKo_2037_CommandPropertySummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IPropertySymbol property && property.Type.IsCommand();

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, string commentXml, IReadOnlyCollection<string> summaries)
        {
            var phrases = GetStartingPhrase((IPropertySymbol)symbol);

            if (summaries.None(_ => _.StartsWithAny(phrases, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Summary, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string[] GetStartingPhrase(IPropertySymbol symbol)
        {
            if (symbol.IsWriteOnly)
            {
                return Constants.Comments.CommandPropertySetterOnlySummaryStartingPhrase;
            }

            if (symbol.IsReadOnly)
            {
                return Constants.Comments.CommandPropertyGetterOnlySummaryStartingPhrase;
            }

            return Constants.Comments.CommandPropertyGetterSetterSummaryStartingPhrase;
        }
    }
}