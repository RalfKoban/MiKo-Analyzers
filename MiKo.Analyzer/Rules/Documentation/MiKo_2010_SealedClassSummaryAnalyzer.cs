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

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsReferenceType && symbol.DeclaredAccessibility == Accessibility.Public && !symbol.IsTestClass();

        protected override  IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => !symbol.IsSealed || summaries.Any(_ => _.EndsWith(Constants.Comments.SealedClassPhrase, StringComparison.Ordinal))
                                                                                                                         ? Enumerable.Empty<Diagnostic>()
                                                                                                                         : new[] { Issue(symbol, Constants.Comments.SealedClassPhrase) };
    }
}