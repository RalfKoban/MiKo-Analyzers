using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2083_NonReadOnlyFieldAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2083";

        public MiKo_2083_NonReadOnlyFieldAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IFieldSymbol field && field.IsReadOnly is false;

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            if (summaryXmls.Any(_ => _.GetTextTrimmed().EndsWith(Constants.Comments.FieldIsReadOnly, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol.Name, summaryXmls[0].EndTag, Constants.Comments.FieldIsReadOnly) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}