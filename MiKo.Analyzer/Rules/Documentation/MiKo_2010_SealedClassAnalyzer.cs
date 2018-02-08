using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2010_SealedClassAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2010";

        internal const string ExpectedComment = "This class cannot be inherited.";

        public MiKo_2010_SealedClassAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsReferenceType && symbol.DeclaredAccessibility == Accessibility.Public;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => AnalyzeSummary(symbol, commentXml);

        protected override  IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => !symbol.IsSealed || summaries.Any(_ => _.EndsWith(ExpectedComment, StringComparison.Ordinal))
                                                                                                                         ? Enumerable.Empty<Diagnostic>()
                                                                                                                         : new[] { ReportIssue(symbol, ExpectedComment) };
    }
}