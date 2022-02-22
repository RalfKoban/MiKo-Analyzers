using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2054_ArgumentExceptionPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2054";

        public MiKo_2054_ArgumentExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentException), true, Constants.Comments.ArgumentExceptionStartingPhrase)
        {
        }
    }
}