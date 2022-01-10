using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2041_InvalidXmlInSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2041";

        public MiKo_2041_InvalidXmlInSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static IEnumerable<XmlNodeSyntax> GetIssues(DocumentationCommentTriviaSyntax documentation) => documentation != null
                                                                                                                    ? documentation.GetSummaryXmls(Constants.Comments.InvalidSummaryCrefXmlTags)
                                                                                                                    : Enumerable.Empty<XmlNodeSyntax>();

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var documentation = symbol.GetDocumentationCommentTriviaSyntax();

            foreach (var node in GetIssues(documentation))
            {
                var name = GetName(node);

                yield return Issue(name, node);
            }
        }

        private static string GetName(SyntaxNode node)
        {
            switch (node)
            {
                case XmlElementSyntax e: return e.GetName();
                case XmlEmptyElementSyntax ee: return ee.GetName();
                default: return string.Empty;
            }
        }
    }
}