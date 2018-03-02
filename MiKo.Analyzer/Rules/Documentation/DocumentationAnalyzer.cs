using System;
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

        protected static IEnumerable<string> GetComments(string commentXml, string xmlElement) => GetCommentElements(commentXml, xmlElement).Select(_ => _.Nodes().ConcatenatedWith().RemoveAll("T:").Trim());

        protected static IEnumerable<XElement> GetCommentElements(string commentXml, string xmlElement)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = "<root>" + commentXml + "</root>";

            try
            {
                return XElement.Parse(xml).Descendants(xmlElement);
            }
            catch (System.Xml.XmlException)
            {
                // invalid character
                return Enumerable.Empty<XElement>();
            }
        }

        protected static string GetCommentForParameter(IParameterSymbol parameter, string commentXml)
        {
            var parameterName = parameter.Name;
            return FlattenComment(GetCommentElements(commentXml, @"param").Where(_ => _.Attribute("name")?.Value == parameterName));
        }

        private static string FlattenComment(IEnumerable<XElement> comments)
        {
            if (!comments.Any()) return null;

            var comment = comments.Nodes().ConcatenatedWith().WithoutParaTags().RemoveAll("T:").Trim();
            return comment;

        }
    }
}