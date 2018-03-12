﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class ReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        protected ReturnTypeDefaultPhraseAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            if (IsAcceptedType(returnType)) return true;

            if (returnType.Name == nameof(System.Threading.Tasks.Task) && TryGetGenericArgumentType(returnType, out var argument))
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

        protected bool TryGetGenericArgumentType(ITypeSymbol symbol, out ITypeSymbol genericArgument, int index = 0)
        {
            genericArgument = null;

            if (symbol is INamedTypeSymbol namedType && namedType.TypeArguments.Length == index + 1)
                genericArgument = namedType.TypeArguments[index];

            return genericArgument != null;
        }
    }
}