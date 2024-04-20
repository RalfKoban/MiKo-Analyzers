using System;

using Microsoft.CodeAnalysis;
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

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.Length > 2 && symbolName[0] == 'T' && symbolName[1] == 'o' && symbolName[2].IsUpperCase())
            {
                // do not report for conversion methods
                return symbolName == "ToString";
            }

            return base.ShallAnalyze(symbol);
        }

        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.IsString();

        protected override string[] GetStartingPhrases(ISymbol owningSymbol, ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                                                        ? Constants.Comments.StringReturnTypeStartingPhrase
                                                                                                        : Constants.Comments.StringTaskReturnTypeStartingPhrase;
    }
}