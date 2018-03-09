﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer : ReturnTypeDefaultPhraseAnalyzer
    {
        public const string Id = "MiKo_2033";

        public MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.Name == nameof(System.String);

        protected override string[] GetStartingPhrases(bool isReturnType) => isReturnType
                                                                                 ? Constants.Comments.StringReturnTypeStartingPhrase
                                                                                 : Constants.Comments.StringTaskReturnTypeStartingPhrase;
    }
}