using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2020";

        private static readonly HashSet<string> AttributeNames = new HashSet<string>
                                                                     {
                                                                         Constants.XmlTag.Attribute.Cref,
                                                                     };

        public MiKo_2020_InheritdocSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml)
        {
            var item = summaryXml.FirstDescendant<XmlNodeSyntax>(_ => _.IsAnyKind(SyntaxKind.XmlEmptyElement, SyntaxKind.XmlElement));

            if (item.IsSee(AttributeNames) || item.IsSeeAlso(AttributeNames))
            {
                // TODO RKN: Enhance to see if it is really an inherited symbol that's linked inside the XML
                return Issue(symbol);
            }

            return null;
        }
    }
}