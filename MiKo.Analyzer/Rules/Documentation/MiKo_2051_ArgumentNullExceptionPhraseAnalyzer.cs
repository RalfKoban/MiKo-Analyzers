﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2051_ArgumentNullExceptionPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2051";

        public MiKo_2051_ArgumentNullExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentNullException), Constants.Comments.ArgumentNullExceptionStartingPhrase)
        {
        }

        protected override IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(ImmutableArray<IParameterSymbol> parameterSymbols) => parameterSymbols.Where(_ => _.Type.IsReferenceType || _.Type.IsNullable()).ToList();
    }
}