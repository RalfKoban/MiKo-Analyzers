using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2055_ArgumentOutOfRangeExceptionPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2055";

        public MiKo_2055_ArgumentOutOfRangeExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentOutOfRangeException), true, Constants.Comments.ArgumentOutOfRangeExceptionStartingPhrase)
        {
        }
    }
}