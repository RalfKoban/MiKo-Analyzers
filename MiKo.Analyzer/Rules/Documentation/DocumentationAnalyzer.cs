using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationAnalyzer : Analyzer
    {
        protected DocumentationAnalyzer(string diagnosticId) : base("Documentation", diagnosticId)
        {
        }

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        //
        // TODO: RKN register other methods, based on symbol kind
        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeClass, SymbolKind.NamedType);

        protected virtual IEnumerable<Diagnostic> AnalyzeClass(INamedTypeSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyzeClass(INamedTypeSymbol symbol) => true;

        protected void AnalyzeClass(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            if (ShallAnalyzeClass(symbol))
            {
                var diagnostics = AnalyzeClass(symbol, symbol.GetDocumentationCommentXml());
                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, string commentXml) => string.IsNullOrWhiteSpace(commentXml)
                                                                                            ? Enumerable.Empty<Diagnostic>()
                                                                                            : AnalyzeSummary(symbol, GetComments(commentXml, "summary"));

        private static IEnumerable<string> GetComments(string commentXml, string xmlElement)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = "<root>" + commentXml + "</root>";

            return XElement.Parse(xml).Descendants(xmlElement).Select(_ => _.Value.Trim());
        }
    }
}