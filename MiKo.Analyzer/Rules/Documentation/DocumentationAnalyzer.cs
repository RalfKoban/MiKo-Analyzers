using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => ShallAnalyzeType(symbol)
                                                                                           ? AnalyzeType(symbol, symbol.GetDocumentationCommentXml())
                                                                                           : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyzeType(INamedTypeSymbol symbol) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyzeMethod(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => ShallAnalyzeMethod(symbol)
                                                                                               ? AnalyzeMethod(symbol, symbol.GetDocumentationCommentXml())
                                                                                               : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol) => AnalyzeSummary(symbol, symbol.GetDocumentationCommentXml());

        protected IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var summaries = GetSummaries(commentXml);
            return summaries.Any() ? AnalyzeSummary(symbol, summaries) : Enumerable.Empty<Diagnostic>();
        }

        protected static ImmutableHashSet<string> GetSummaries(string commentXml) => GetComments(commentXml, "summary").WithoutParaTags().Select(_ => _.Trim()).ToImmutableHashSet();

        protected static IEnumerable<string> GetComments(string commentXml, string xmlElement) => GetCommentElements(commentXml, xmlElement).Select(_ => _.Nodes().ConcatenatedWith().RemoveAll("T:").Trim());

        protected static IEnumerable<XElement> GetCommentElements(string commentXml, string xmlElement)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = "<root>" + commentXml + "</root>";

            return XElement.Parse(xml).Descendants(xmlElement);
        }

        protected static string GetCommentForParameter(IParameterSymbol parameter, string commentXml)
        {
            var paramElements = GetCommentElements(commentXml, @"param");
            var comments = paramElements.Where(_ => _.Attribute("name")?.Value == parameter.Name);
            var comment = comments.Nodes().ConcatenatedWith().WithoutParaTags().RemoveAll("T:").Trim();
            return comment;
        }
    }
}