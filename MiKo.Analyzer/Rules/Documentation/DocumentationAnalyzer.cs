using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected DocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(nameof(Documentation), diagnosticId, symbolKind)
        {
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyzeType(INamedTypeSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => ShallAnalyzeType(symbol)
                                                                                           ? AnalyzeType(symbol, symbol.GetDocumentationCommentXml())
                                                                                           : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, string commentXml) => string.IsNullOrWhiteSpace(commentXml)
                                                                                               ? Enumerable.Empty<Diagnostic>()
                                                                                               : AnalyzeSummary(symbol, GetComments(commentXml, "summary"));

        protected static IEnumerable<string> GetComments(string commentXml, string xmlElement) => GetCommentElements(commentXml, xmlElement).Select(_ => _.Value.Trim());

        protected static IEnumerable<XElement> GetCommentElements(string commentXml, string xmlElement)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = "<root>" + commentXml + "</root>";

            return XElement.Parse(xml).Descendants(xmlElement);
        }
    }
}