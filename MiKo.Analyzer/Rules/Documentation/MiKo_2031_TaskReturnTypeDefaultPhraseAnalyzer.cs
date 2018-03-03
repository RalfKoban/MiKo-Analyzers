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

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(IMethodSymbol method, string comment, string xmlTag)
        {
            var returnType = method.ReturnType;

            if (returnType is INamedTypeSymbol namedType && namedType.TypeArguments.Length > 0)
            {
                // we have a generic task
                return GenericTypeAccepted(namedType.TypeArguments[0].Name)
                           ? AnalyzeStartingPhrase(method, comment, xmlTag, Constants.Comments.ReturnTypeTaskWithResultStartingPhrase)
                           : Enumerable.Empty<Diagnostic>();
            }

            return AnalyzePhrase(method, comment, xmlTag, Constants.Comments.ReturnTypeTaskWithoutResultPhrase);
        }

        private static bool GenericTypeAccepted(string name)
        {
            switch (name)
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