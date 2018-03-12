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
            var endingPhrases = GeEndingPhrases(returnType);

            return comment.StartsWithAny(StringComparison.Ordinal, startingPhrases) && comment.EndsWithAny(StringComparison.Ordinal, endingPhrases)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(owningSymbol, owningSymbol.Name, xmlTag, startingPhrases[0], endingPhrases[0]) };
        }

        // ReSharper disable once RedundantNameQualifier
        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.SpecialType == SpecialType.System_Boolean;

        protected override string[] GetStartingPhrases(ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                                     ? Constants.Comments.BooleanReturnTypeStartingPhrase
                                                                                     : Constants.Comments.BooleanTaskReturnTypeStartingPhrase;

        private string[] GeEndingPhrases(ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                        ? Constants.Comments.BooleanReturnTypeEndingPhrase
                                                                        : Constants.Comments.BooleanTaskReturnTypeEndingPhrase;

    }
}