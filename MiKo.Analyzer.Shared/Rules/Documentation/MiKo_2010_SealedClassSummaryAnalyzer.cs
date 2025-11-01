using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2010_SealedClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2010";

        public MiKo_2010_SealedClassSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol type && type.IsReferenceType && type.DeclaredAccessibility is Accessibility.Public && type.IsTestClass() is false;

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            if (symbol.IsSealed && summaryXmls.None(_ => _.GetTextTrimmed().EndsWith(Constants.Comments.SealedClassPhrase, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol.Name, summaryXmls[0].EndTag, Constants.Comments.SealedClassPhrase) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}