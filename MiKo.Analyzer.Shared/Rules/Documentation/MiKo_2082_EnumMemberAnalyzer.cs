using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2082_EnumMemberAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2082";

        private static readonly string[] StartingPhrases =
            {
                "Defines",
                "Indicates",
                "Specifies",
            };

        public MiKo_2082_EnumMemberAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        // overridden because we want to inspect the fields of the type as well
        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsEnum())
            {
                return symbol.GetFields()
                             .Where(ShallAnalyze)
                             .SelectMany(_ => AnalyzeSummaries(_, compilation, _.GetDocumentationCommentXml(), _.GetDocumentationCommentTriviaSyntax()));
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment) => from summary in summaries
                                                                                                                                                                                       where summary.StartsWithAny(StartingPhrases, StringComparison.OrdinalIgnoreCase)
                                                                                                                                                                                       select Issue(symbol, summary.FirstWord());
    }
}