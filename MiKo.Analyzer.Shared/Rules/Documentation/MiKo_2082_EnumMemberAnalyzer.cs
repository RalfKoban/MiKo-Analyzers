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

        public MiKo_2082_EnumMemberAnalyzer() : base(Id, SymbolKind.NamedType)
        {
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
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeSummariesStart(symbol, compilation, commentXml, comment);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.OrdinalIgnoreCase;

            var text = valueText.AsSpan().TrimStart();

            if (text.StartsWithAny(Constants.Comments.EnumMemberWrongStartingWords, comparison))
            {
                problematicText = text.FirstWord().ToString();

                return true;
            }

            return false;
        }
    }
}