using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer : ReturnTypeDefaultPhraseAnalyzer
    {
        public const string Id = "MiKo_2035";

        public MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.IsEnumerable();

        protected override string[] GetStartingPhrases(bool isReturnType) => isReturnType
                                                                                 ? Constants.Comments.EnumerableReturnTypeStartingPhrase
                                                                                 : Constants.Comments.EnumerableTaskReturnTypeStartingPhrase;
    }
}