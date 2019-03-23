using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        protected ReturnTypeDefaultPhraseAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            if (IsAcceptedType(returnType)) return true;

            if (returnType.IsTask() && returnType.TryGetGenericArgumentType(out var argument))
            {
                // we have a generic task
                return IsAcceptedType(argument);
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            var startingPhrases = GetStartingPhrases(returnType);

            return AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, startingPhrases);
        }

        protected abstract bool IsAcceptedType(ITypeSymbol returnType);

        protected abstract string[] GetStartingPhrases(ITypeSymbol returnType);
    }
}