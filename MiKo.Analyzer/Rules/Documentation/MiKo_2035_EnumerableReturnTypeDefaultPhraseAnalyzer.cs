using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer : ReturnTypeDefaultPhraseAnalyzer
    {
        public const string Id = "MiKo_2035";

        private static readonly string[] Empty = new string[0];

        public MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.IsEnumerable();

        protected override string[] GetStartingPhrases(ITypeSymbol returnType)
        {
            if (returnType.IsEnumerable())
            {
                return returnType.Kind == SymbolKind.ArrayType
                           ? Constants.Comments.ArrayReturnTypeStartingPhrase
                           : Constants.Comments.EnumerableReturnTypeStartingPhrase;
            }

            if (TryGetGenericArgumentType(returnType, out var argument))
            {
                return argument.Kind == SymbolKind.ArrayType
                           ? Constants.Comments.ArrayTaskReturnTypeStartingPhrase
                           : Constants.Comments.EnumerableTaskReturnTypeStartingPhrase;
            }

            return Empty; // should never happen
        }
    }
}