using System;
using System.Collections.Generic;

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

        protected override Diagnostic StartIssue(SyntaxNode node)
        {
            var location = node.GetLocation();

            return Issue(location);
        }

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(location);

        // overridden because we want to inspect the fields of the type as well
        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsEnum())
            {
                foreach (var field in symbol.GetFields())
                {
                    foreach (var issue in AnalyzeField(field, compilation))
                    {
                        yield return issue;
                    }
                }
            }
        }

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeTextStart(symbol, summaryXml);
            }
        }

        protected override bool AnalyzeTextStart(string valueText, out string problematicText)
        {
            var text = valueText.AsSpan().TrimStart();

            var startsWith = text.StartsWithAny(StartingPhrases, StringComparison.OrdinalIgnoreCase);

            problematicText = text.FirstWord().ToString();

            return startsWith;
        }
    }
}