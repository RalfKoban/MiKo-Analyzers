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

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsReferenceType
                                                                      && symbol.IsSealed
                                                                      && symbol.DeclaredAccessibility == Accessibility.Public
                                                                      && symbol.IsTestClass() is false
                                                                      && base.ShallAnalyze(symbol);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeEnd(symbol, summaryXml, Constants.Comments.SealedClassPhrase);
    }
}