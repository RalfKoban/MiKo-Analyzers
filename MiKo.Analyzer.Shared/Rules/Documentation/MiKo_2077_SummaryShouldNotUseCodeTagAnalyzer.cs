using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2077_SummaryShouldNotUseCodeTagAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2077";

        public MiKo_2077_SummaryShouldNotUseCodeTagAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, string commentXml, IReadOnlyCollection<string> summaries)
        {
            var count = summaryXmls.Count;

            List<Diagnostic> issues = null;

            for (var index = 0; index < count; index++)
            {
                var summaryXml = summaryXmls[index];

                foreach (var code in summaryXml.GetXmlSyntax(Constants.XmlTag.Code))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    // we have an issue
                    issues.Add(Issue(symbol.Name, code));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}