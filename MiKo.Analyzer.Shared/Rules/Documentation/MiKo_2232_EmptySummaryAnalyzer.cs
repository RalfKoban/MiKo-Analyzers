using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2232_EmptySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2232";

        public MiKo_2232_EmptySummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var summary in comment.GetSummaryXmls())
            {
                var content = summary.Content;

                switch (content.Count)
                {
                    case 0:
                    case 1 when content[0] is XmlTextSyntax text && text.GetTextWithoutTrivia().IsEmpty:
                        return new[] { Issue(summary) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}