﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2090_EqualityOperatorAnalyzer : OperatorAnalyzer
    {
        public const string Id = "MiKo_2090";

        private static readonly string[] ReturnsPhrases =
                                                          {
                                                              "<see langword=\"true\"/> if both instances are considered equal; otherwise, <see langword=\"false\"/>.",
                                                              "<see langword=\"true\"/> if both instances are considered equal; otherwise, <see langword=\"false\" />.",
                                                              "<see langword=\"true\" /> if both instances are considered equal; otherwise, <see langword=\"false\"/>.",
                                                              "<see langword=\"true\" /> if both instances are considered equal; otherwise, <see langword=\"false\" />.",
                                                          };

        public MiKo_2090_EqualityOperatorAnalyzer() : base(Id, "op_Equality")
        {
        }

        protected override string[] GetSummaryPhrases(ISymbol symbol)
        {
            var containingType = symbol.ContainingType;
            var containingTypeName = containingType.Name;
            var fullyQualifiedName = containingType.FullyQualifiedName();

            return new[]
                       {
                           "Determines whether the specified <see cref=\"" + containingTypeName + "\"/> instances are considered equal.",
                           "Determines whether the specified <see cref=\"" + containingTypeName + "\" /> instances are considered equal.",
                           "Determines whether the specified <see cref=\"" + fullyQualifiedName + "\"/> instances are considered equal.",
                           "Determines whether the specified <see cref=\"" + fullyQualifiedName + "\" /> instances are considered equal.",
                       };
        }

        protected override string[] GetReturnsPhrases(ISymbol symbol) => ReturnsPhrases;
    }
}