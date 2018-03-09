using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2034";

        public MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            if (returnType.IsEnum()) return true;

            if (returnType.Name == nameof(System.Threading.Tasks.Task) && (returnType is INamedTypeSymbol namedType && namedType.TypeArguments.Length == 1))
            {
                // we have a generic task
                return namedType.TypeArguments[0].IsEnum();
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            var isEnum = returnType.IsEnum();

            var startingPhrases = isEnum ? Constants.Comments.EnumReturnTypeStartingPhrase : Constants.Comments.EnumTaskReturnTypeStartingPhrase;

            return AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, startingPhrases);
        }
    }
}