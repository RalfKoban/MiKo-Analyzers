using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2040_LangwordAnalyzer : DocumentationAnalyzer
    {
        private static readonly KeyValuePair<string, string>[] Items  = new [] { "null", "true", "false" }.Select(_ => new KeyValuePair<string, string>("<c>" + _ + "</c>", "<see langword=\"" + _ + "\" />")).ToArray();

        public const string Id = "MiKo_2040";

        public MiKo_2040_LangwordAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }


        public override void Initialize(AnalysisContext context)
        {
            Initialize(context, SymbolKind.Event);
            Initialize(context, SymbolKind.Field);
            Initialize(context, SymbolKind.Method);
            Initialize(context, SymbolKind.NamedType);
            Initialize(context, SymbolKind.Property);
        }

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => AnalyzeComment(symbol);

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol)
        {
            List<Diagnostic> findings = null;

            var comment = symbol.GetDocumentationCommentXml();
            foreach (var item in Items.Where(_ => comment.Contains(_.Key, StringComparison.OrdinalIgnoreCase)))
            {
                if (findings == null) findings = new List<Diagnostic>();
                findings.Add(ReportIssue(symbol, item.Key, item.Value));
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }
    }
}