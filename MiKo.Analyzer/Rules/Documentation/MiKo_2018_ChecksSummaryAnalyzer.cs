using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2018_ChecksSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2018";

        internal const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        private static readonly string[] WrongPhrases = { "Check ", "Checks ", "Test ", "Tests ", "Determines if " };

        public MiKo_2018_ChecksSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && base.ShallAnalyze(symbol);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var trimmed = summary
                          .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                          .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                          .Trim();

            foreach (var wrongPhrase in WrongPhrases)
            {
                if (trimmed.StartsWith(wrongPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    var location = GetFirstLocation(textToken, wrongPhrase, StringComparison.OrdinalIgnoreCase);

                    return Issue(symbol.Name, location, wrongPhrase, StartingPhrase);
                }
            }

            return null;
        }
    }
}