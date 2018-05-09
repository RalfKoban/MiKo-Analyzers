using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2052_ArgumentNullExceptionPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2052";

        public MiKo_2052_ArgumentNullExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentNullException), false, Constants.Comments.ArgumentNullExceptionStartingPhrase)
        {
        }

        protected override IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(IReadOnlyCollection<IParameterSymbol> parameterSymbols) => parameterSymbols.Where(_ => _.Type.IsReferenceType || _.Type.IsNullable()).ToList();
    }
}