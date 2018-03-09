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

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType) => returnType.Name == nameof(System.Threading.Tasks.Task);

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            if (returnType is INamedTypeSymbol namedType && namedType.TypeArguments.Length > 0)
            {
                // we have a generic task
                return GenericTypeAccepted(namedType.TypeArguments[0])
                           ? AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, Constants.Comments.GenericTaskReturnTypeStartingPhrase)
                           : Enumerable.Empty<Diagnostic>();
            }

            return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.NonGenericTaskReturnTypePhrase);
        }

        private static bool GenericTypeAccepted(ITypeSymbol returnType)
        {
            if (returnType.IsEnum()) return false; // checked by MiKo_2034
            if (returnType.IsEnumerable()) return false; // checked by MiKo_2035

            switch (returnType.Name)
            {
                // ReSharper disable RedundantNameQualifier
                case nameof(System.Boolean): // checked by MiKo_2032
                case nameof(System.String):  // checked by MiKo_2033
                    return false;

                // ReSharper restore RedundantNameQualifier
                default:
                    return true;
            }
        }
    }
}