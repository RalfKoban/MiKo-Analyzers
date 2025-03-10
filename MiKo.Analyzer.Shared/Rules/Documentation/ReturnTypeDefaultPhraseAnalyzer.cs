﻿using Microsoft.CodeAnalysis;
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

        protected override Diagnostic[] AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag)
        {
            var startingPhrases = GetStartingPhrases(owningSymbol, returnType);

            return AnalyzeStartingPhrase(owningSymbol, comment, commentXml, xmlTag, startingPhrases);
        }

        protected abstract bool IsAcceptedType(ITypeSymbol returnType);

        protected abstract string[] GetStartingPhrases(ISymbol owningSymbol, ITypeSymbol returnType);
    }
}