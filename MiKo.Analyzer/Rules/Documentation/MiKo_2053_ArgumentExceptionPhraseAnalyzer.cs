using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2053_ArgumentExceptionPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2053";

        public MiKo_2053_ArgumentExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentException), Constants.Comments.ArgumentExceptionStartingPhrase)
        {
        }
    }
}