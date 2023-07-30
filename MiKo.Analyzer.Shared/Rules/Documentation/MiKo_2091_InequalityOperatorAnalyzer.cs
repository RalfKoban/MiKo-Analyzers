using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2091_InequalityOperatorAnalyzer : OperatorAnalyzer
    {
        public const string Id = "MiKo_2091";

        private static readonly string[] ReturnsPhrases =
                                                          {
                                                              "<see langword=\"true\"/> if both instances are considered not equal; otherwise, <see langword=\"false\"/>.",
                                                              "<see langword=\"true\"/> if both instances are considered not equal; otherwise, <see langword=\"false\" />.",
                                                              "<see langword=\"true\" /> if both instances are considered not equal; otherwise, <see langword=\"false\"/>.",
                                                              "<see langword=\"true\" /> if both instances are considered not equal; otherwise, <see langword=\"false\" />.",
                                                          };

        public MiKo_2091_InequalityOperatorAnalyzer() : base(Id, "op_Inequality")
        {
        }

        protected override string[] GetSummaryPhrases(ISymbol symbol) => new[]
                                                                             {
                                                                                 $"Determines whether the specified <see cref=\"{symbol.ContainingType.Name}\"/> instances are considered not equal.",
                                                                                 $"Determines whether the specified <see cref=\"{symbol.ContainingType.Name}\" /> instances are considered not equal.",
                                                                                 $"Determines whether the specified <see cref=\"{symbol.ContainingType.FullyQualifiedName()}\"/> instances are considered not equal.",
                                                                                 $"Determines whether the specified <see cref=\"{symbol.ContainingType.FullyQualifiedName()}\" /> instances are considered not equal.",
                                                                             };

        protected override string[] GetReturnsPhrases(ISymbol symbol) => ReturnsPhrases;
    }
}