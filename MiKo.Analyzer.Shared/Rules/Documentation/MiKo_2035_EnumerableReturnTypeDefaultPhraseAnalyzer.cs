using System;
using System.Linq;

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

        protected override string[] GetStartingPhrases(ISymbol owningSymbol, ITypeSymbol returnType)
        {
            if (returnType.IsEnumerable())
            {
                var initialPhrases = GetNonGenericInitialPhrases(returnType);

                return GetStartingPhrases(returnType, initialPhrases).ToArray();
            }

            if (returnType.TryGetGenericArgumentType(out var argumentType))
            {
                if (argumentType is IArrayTypeSymbol array)
                {
                    return array.ElementType.IsByte()
                           ? Constants.Comments.ByteArrayTaskReturnTypeStartingPhrase
                           : Constants.Comments.ArrayTaskReturnTypeStartingPhrase;
                }

                return Constants.Comments.EnumerableTaskReturnTypeStartingPhrase;
            }

            return Array.Empty<string>(); // should never happen
        }

        private static string[] GetNonGenericInitialPhrases(ITypeSymbol returnType)
        {
            if (returnType is IArrayTypeSymbol arrayType)
            {
                return arrayType.ElementType.IsByte()
                       ? Constants.Comments.ByteArrayReturnTypeStartingPhrase
                       : Constants.Comments.ArrayReturnTypeStartingPhrase;
            }

            return Constants.Comments.EnumerableReturnTypeStartingPhrase;
        }
    }
}