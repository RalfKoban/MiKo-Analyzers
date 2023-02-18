using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        protected ReturnTypeDefaultPhraseAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            if (IsAcceptedType(returnType))
            {
                return true;
            }

            if (returnType.IsTask() && returnType.TryGetGenericArgumentType(out var argumentType))
            {
                // we have a generic task
                return IsAcceptedType(argumentType);
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string commentXml, string xmlTag, DocumentationCommentTriviaSyntax comment)
        {
            var startingPhrases = GetStartingPhrases(owningSymbol, returnType);

            return AnalyzeStartingPhrase(owningSymbol, commentXml, xmlTag, startingPhrases, comment);
        }

        protected abstract bool IsAcceptedType(ITypeSymbol returnType);

        protected abstract string[] GetStartingPhrases(ISymbol owningSymbol, ITypeSymbol returnType);
    }
}