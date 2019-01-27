using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2203_DocumentationShallUseListAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2203";

        private static readonly string[] Delimiters = { ".)", ".", ")", ":" };

        private static readonly string[] Triggers = Enumerable.Concat(
                                                      new[] { "-", "--", "---" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => delimiter + " " + _ + " "),
                                                      new[] { "1", "a", "2", "b", "3", "c" }.SelectMany(_ => Delimiters, (_, delimiter) => " " + _ + delimiter + " ")
                                                      ).ToArray();

        public MiKo_2203_DocumentationShallUseListAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property, SymbolKind.TypeParameter);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(Triggers, StringComparison.OrdinalIgnoreCase)
                                                                                                            ? new[] { ReportIssue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}