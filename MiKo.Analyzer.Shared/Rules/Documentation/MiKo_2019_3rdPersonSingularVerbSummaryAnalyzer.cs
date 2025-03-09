using System;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2019";

        public MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type:
                    return type.IsNamespace is false && type.IsEnum() is false && type.IsException() is false;
                case IMethodSymbol _:
                case IPropertySymbol _:
                    return true;

                default:
                    return false;
            }
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.Ordinal;

            var builder = valueText.AsCachedBuilder()
                                   .Without(Constants.Comments.AsynchronouslyStartingPhrase) // skip over async starting phrase
                                   .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                                   .Without(","); // skip over first comma

            problematicText = builder.FirstWord(out _);

            StringBuilderCache.Release(builder);

            return Verbalizer.IsThirdPersonSingularVerb(problematicText) is false;
        }
    }
}