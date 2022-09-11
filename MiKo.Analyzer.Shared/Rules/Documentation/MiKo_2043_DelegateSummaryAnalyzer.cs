using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2043_DelegateSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2043";

        private const string Phrase = Constants.Comments.DelegateSummaryStartingPhrase;

        public MiKo_2043_DelegateSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Delegate && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries) => summaries.Any(_ => _.Contains(Phrase))
                                                                                                                                                 ? Enumerable.Empty<Diagnostic>()
                                                                                                                                                 : new[] { Issue(symbol, Constants.XmlTag.Summary, Phrase) };
    }
}