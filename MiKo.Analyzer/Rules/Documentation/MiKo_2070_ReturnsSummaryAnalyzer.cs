using System;
using System.Collections;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2070";

        internal static readonly string[] Phrases = { "Return", "Returns" };

        public MiKo_2070_ReturnsSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            switch (symbol.Name)
            {
                case nameof(ToString):
                case nameof(IEnumerable.GetEnumerator):
                    return false;

                default:
                    return base.ShallAnalyze(symbol);
            }
        }

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, GetProposal(symbol));

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var trimmed = summary
                          .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                          .TrimStart();

            var firstWord = trimmed.FirstWord();

            foreach (var word in Phrases)
            {
                if (word.Equals(firstWord, StringComparison.OrdinalIgnoreCase))
                {
                    var location = GetFirstLocation(textToken, firstWord);

                    return Issue(symbol.Name, location, GetProposal(symbol));
                }
            }

            return null;
        }

        private static string GetProposal(ISymbol symbol)
        {
            if (symbol is IMethodSymbol m && m.ReturnType.IsBoolean())
            {
                return Constants.Comments.DeterminesWhetherPhrase;
            }

            return "Gets";
        }
    }
}