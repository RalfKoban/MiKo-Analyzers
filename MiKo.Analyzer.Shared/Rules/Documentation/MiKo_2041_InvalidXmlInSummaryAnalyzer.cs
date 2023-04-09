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

        private static readonly HashSet<string> Tags = Constants.Comments.InvalidSummaryCrefXmlTags.ToHashSet();


        public MiKo_2041_InvalidXmlInSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static IEnumerable<XmlNodeSyntax> GetIssues(DocumentationCommentTriviaSyntax documentation) => documentation?.GetSummaryXmls(Tags) ?? Enumerable.Empty<XmlNodeSyntax>();

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var node in GetIssues(comment))
            {
                var name = node.GetXmlTagName();

                yield return Issue(name, node);
            }
        }
    }
}