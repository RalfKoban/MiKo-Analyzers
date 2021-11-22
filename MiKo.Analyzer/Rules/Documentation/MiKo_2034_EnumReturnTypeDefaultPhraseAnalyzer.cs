using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer : ReturnTypeDefaultPhraseAnalyzer
    {
        public const string Id = "MiKo_2034";

        public MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.IsEnum();

        protected override string[] GetStartingPhrases(ISymbol owningSymbol, ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                                                            ? Constants.Comments.EnumReturnTypeStartingPhrase
                                                                                                            : Constants.Comments.EnumTaskReturnTypeStartingPhrase;
    }
}