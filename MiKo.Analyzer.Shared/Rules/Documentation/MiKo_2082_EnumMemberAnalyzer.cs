using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2082_EnumMemberAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2082";

        public MiKo_2082_EnumMemberAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IFieldSymbol field && field.ContainingType.IsEnum();

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(location);

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