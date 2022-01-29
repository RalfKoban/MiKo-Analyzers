using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2010_SealedClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2010";

        public MiKo_2010_SealedClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsReferenceType && symbol.DeclaredAccessibility == Accessibility.Public && symbol.IsTestClass() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => symbol.IsSealed is false || summaries.Any(_ => _.EndsWith(Constants.Comments.SealedClassPhrase, StringComparison.Ordinal))
                                                                                                                         ? Enumerable.Empty<Diagnostic>()
                                                                                                                         : new[] { Issue(symbol, Constants.Comments.SealedClassPhrase) };
    }
}