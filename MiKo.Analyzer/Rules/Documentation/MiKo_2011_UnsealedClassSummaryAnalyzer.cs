using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2011_UnsealedClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2011";

        public MiKo_2011_UnsealedClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsReferenceType
                                                                      && symbol.IsSealed is false
                                                                      && symbol.DeclaredAccessibility == Accessibility.Public
                                                                      && symbol.IsTestClass() is false
                                                                      && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<XmlElementSyntax> summaryXmls)
        {
            return AnalyzeSummaryContains(symbol, summaryXmls, new[] { Constants.Comments.SealedClassPhrase }, StringComparison.Ordinal);
        }
    }
}