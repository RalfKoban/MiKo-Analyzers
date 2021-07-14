using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2031";

        public MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType) => returnType.IsTask();

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            if (returnType.TryGetGenericArgumentType(out var argumentType))
            {
                // we have a generic task
                return GenericTypeAccepted(argumentType)
                           ? AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, Constants.Comments.GenericTaskReturnTypeStartingPhrase)
                           : Enumerable.Empty<Diagnostic>();
            }

            return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.NonGenericTaskReturnTypePhrase);
        }

        private static bool GenericTypeAccepted(ITypeSymbol returnType)
        {
            if (returnType.IsBoolean())
            {
                return false; // checked by MiKo_2032
            }

            if (returnType.IsString())
            {
                return false; // checked by MiKo_2033
            }

            if (returnType.IsEnum())
            {
                return false; // checked by MiKo_2034
            }

            if (returnType.IsEnumerable())
            {
                return false; // checked by MiKo_2035
            }

            return true;
        }
    }
}