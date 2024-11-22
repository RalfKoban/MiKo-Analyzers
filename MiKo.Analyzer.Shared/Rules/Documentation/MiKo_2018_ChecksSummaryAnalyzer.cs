using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2018_ChecksSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2018";

        private const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        private static readonly string[] WrongPhrases = { "Check ", "Checks ", "Test ", "Tests ", "Determines if " };

        public MiKo_2018_ChecksSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ConsiderEmptyTextAsIssue(ISymbol symbol) => symbol is IMethodSymbol method && method.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                if (symbol is ITypeSymbol && summaryXml.GetTextTrimmed().IsNullOrEmpty())
                {
                    // do not report for empty types
                    continue;
                }

                yield return AnalyzeTextStart(symbol, summaryXml);
            }
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.OrdinalIgnoreCase;

            var trimmedSummary = valueText.AsBuilder()
                                          .Without(Constants.Comments.AsynchronouslyStartingPhrase) // skip over async starting phrase
                                          .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                                          .Without(",") // skip over first comma
                                          .TrimStart();

            foreach (var wrongPhrase in WrongPhrases)
            {
                if (trimmedSummary.StartsWith(wrongPhrase, comparison))
                {
                    problematicText = wrongPhrase.TrimEnd();

                    return true;
                }
            }

            problematicText = null;

            return false;
        }
    }
}