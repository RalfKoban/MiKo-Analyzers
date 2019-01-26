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

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property, SymbolKind.TypeParameter);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.StartsWith("<!--", StringComparison.OrdinalIgnoreCase)
                                                                                                            ? new[] { ReportIssue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}