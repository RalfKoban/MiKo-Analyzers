using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2081_ReadOnlyFieldAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2081";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2081_ReadOnlyFieldAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            if (symbol is IFieldSymbol field && field.IsReadOnly)
            {
                switch (field.DeclaredAccessibility)
                {
                    case Accessibility.Public:
                    case Accessibility.Protected:
                        return field.ContainingType.IsTestClass() is false;
                }
            }

            return false;
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<IReadOnlyCollection<string>> summaries)
        {
            if (summaries.Value.None(_ => _.EndsWith(Constants.Comments.FieldIsReadOnly, Comparison)))
            {
                return new[] { Issue(symbol, Constants.Comments.FieldIsReadOnly) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}