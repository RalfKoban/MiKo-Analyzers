using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2019";

        public MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && symbol.IsException() is false && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var trimmed = summary
                          .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                          .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                          .Trim();

            var firstWord = trimmed.FirstWord();

            if (Verbalizer.IsThirdPersonSingularVerb(firstWord))
            {
                return null;
            }

            var location = GetFirstLocation(textToken, firstWord);

            return Issue(symbol.Name, location);
        }
    }
}