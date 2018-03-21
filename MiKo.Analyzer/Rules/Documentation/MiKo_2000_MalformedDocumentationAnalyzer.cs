using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2000_MalformedDocumentationAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2000";

        public MiKo_2000_MalformedDocumentationAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => AnalyzeComment(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => AnalyzeComment(symbol);

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol)
        {
            var comment = symbol.GetDocumentationCommentXml();
            return comment.StartsWith("<!--", StringComparison.OrdinalIgnoreCase)
                       ? new[] { ReportIssue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}