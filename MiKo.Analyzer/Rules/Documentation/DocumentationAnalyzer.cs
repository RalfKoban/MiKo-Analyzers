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

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => ShallAnalyzeMethod(symbol)
                                                                                              ? AnalyzeMethod(symbol, symbol.GetDocumentationCommentXml())
                                                                                              : Enumerable.Empty<Diagnostic>();


        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => ShallAnalyzeEvent(symbol)
                                                                                            ? AnalyzeEvent(symbol, symbol.GetDocumentationCommentXml())
                                                                                            : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => ShallAnalyzeField(symbol)
                                                                                            ? AnalyzeField(symbol, symbol.GetDocumentationCommentXml())
                                                                                            : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => ShallAnalyzeProperty(symbol)
                                                                                                  ? AnalyzeProperty(symbol, symbol.GetDocumentationCommentXml())
                                                                                                  : Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyzeType(INamedTypeSymbol symbol) => true;

        protected virtual bool ShallAnalyzeMethod(IMethodSymbol symbol) => true;

        protected virtual bool ShallAnalyzeEvent(IEventSymbol symbol) => true;

        protected virtual bool ShallAnalyzeField(IFieldSymbol symbol) => true;

        protected virtual bool ShallAnalyzeProperty(IPropertySymbol symbol) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected static IEnumerable<string> GetComments(string commentXml, string xmlElement) => Cleaned(GetCommentElements(commentXml, xmlElement));

        protected static IEnumerable<string> GetComments(string commentXml, string xmlElement, string xmlSubElement) => Cleaned(GetCommentElements(commentXml, xmlElement).Descendants(xmlSubElement));

        protected static ImmutableHashSet<string> GetRemarks(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Remarks));

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

        protected static string GetParameterComment(IParameterSymbol parameter, string commentXml)
        {
            var parameterName = parameter.Name;
            return FlattenComment(GetCommentElements(commentXml, Constants.XmlTag.Param).Where(_ => _.Attribute("name")?.Value == parameterName));
        }

        private static string FlattenComment(IEnumerable<XElement> comments)
        {
            if (!comments.Any()) return null;

            var comment = comments.Nodes().ConcatenatedWith().WithoutParaTags().RemoveAll(Constants.SymbolMarkersAndLineBreaks).Trim();
            return comment
                   .Replace("    ", " ")
                   .Replace("   ", " ")
                   .Replace("  ", " ");
        }

        protected static ImmutableHashSet<string> Cleaned(IEnumerable<string> comments) => comments.WithoutParaTags().Select(_ => _.Trim()).ToImmutableHashSet();

        private static IEnumerable<string> Cleaned(IEnumerable<XElement> elements) => elements.Select(_ => _.Nodes()
                                                                                                            .ConcatenatedWith()
                                                                                                            .RemoveAll(Constants.SymbolMarkersAndLineBreaks)
                                                                                                            .Replace("    ", " ")
                                                                                                            .Trim());
    }
}