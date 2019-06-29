using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer : ReturnTypeDefaultPhraseAnalyzer
    {
        public const string Id = "MiKo_2032";

        public MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            var startingPhrases = GetStartingPhrases(returnType);
            var endingPhrases = GetEndingPhrases(returnType);

            const StringComparison Comparison = StringComparison.Ordinal;

            return comment.StartsWithAny(startingPhrases, Comparison) && comment.ContainsAny(endingPhrases, Comparison)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(owningSymbol, xmlTag, startingPhrases[0], endingPhrases[0]) };
        }

        // ReSharper disable once RedundantNameQualifier
        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.SpecialType == SpecialType.System_Boolean;

        protected override string[] GetStartingPhrases(ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                                     ? Constants.Comments.BooleanReturnTypeStartingPhrase
                                                                                     : Constants.Comments.BooleanTaskReturnTypeStartingPhrase;

        private string[] GetEndingPhrases(ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                        ? Constants.Comments.BooleanReturnTypeEndingPhrase
                                                                        : Constants.Comments.BooleanTaskReturnTypeEndingPhrase;
    }
}