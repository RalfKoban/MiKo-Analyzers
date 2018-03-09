using System.Collections.Generic;

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

            if (returnType.Name == nameof(System.Threading.Tasks.Task) && (returnType is INamedTypeSymbol namedType && namedType.TypeArguments.Length == 1))
            {
                // we have a generic task
                return IsAcceptedType(namedType.TypeArguments[0]);
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            var startingPhrases = GetStartingPhrases(IsAcceptedType(returnType));

            return AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, startingPhrases);
        }

        protected abstract bool IsAcceptedType(ITypeSymbol returnType);

        protected abstract string[] GetStartingPhrases(bool isReturnType);
    }
}