using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2030_ReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2030";

        public MiKo_2030_ReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            if (returnType.IsEnum()) return false; // checked by MiKo_2034

            switch (returnType.Name)
            {
                // ReSharper disable RedundantNameQualifier
                case nameof(System.Threading.Tasks.Task):   // checked by MiKo_2031, MiKo_2032, MiKo_2033
                case nameof(System.Boolean):                // checked by MiKo_2032
                case nameof(System.String):                 // checked by MiKo_2033
                    return false;

                // ReSharper restore RedundantNameQualifier
                default:
                    return true;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(IMethodSymbol method, string comment, string xmlTag) => AnalyzeStartingPhrase(method, comment, xmlTag, Constants.Comments.ReturnTypeStartingPhrase);
    }
}