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

        public MiKo_2037_CommandPropertySummaryAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.Type.IsCommand() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetStartingPhrase((IPropertySymbol)symbol);

            if (summaries.None(_ => _.StartsWithAny(phrases, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Summary, phrases[0]) };
            }

            return Enumerable.Empty<Diagnostic>();
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