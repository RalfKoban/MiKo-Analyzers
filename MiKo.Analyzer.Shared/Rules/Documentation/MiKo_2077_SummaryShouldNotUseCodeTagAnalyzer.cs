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

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            return Analyze(symbol, summaryXmls);
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeContents(
                                                                 DocumentationCommentTriviaSyntax comment,
                                                                 ISymbol symbol,
                                                                 IReadOnlyList<XmlElementSyntax> contentXmls,
                                                                 Lazy<string> commentXml,
                                                                 Lazy<string[]> contents)
        {
            return Analyze(symbol, contentXmls);
        }

        private Diagnostic[] Analyze(ISymbol symbol, IReadOnlyList<XmlElementSyntax> xmls)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, count = xmls.Count; index < count; index++)
            {
                var xml = xmls[index];

                foreach (var code in xml.GetXmlSyntax(Constants.XmlTag.Code))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    // we have an issue
                    issues.Add(Issue(symbol.Name, code));
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}