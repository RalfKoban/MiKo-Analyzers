using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2030_ReturnValueDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2030";

        public MiKo_2030_ReturnValueDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnValue(ITypeSymbol returnValue)
        {
            if (returnValue.IsEnum()) return false;

            switch (returnValue.Name)
            {
                // ReSharper disable RedundantNameQualifier
                case nameof(System.Boolean):
                case nameof(System.String):
                    return false;

                // ReSharper restore RedundantNameQualifier
                default:
                    return true;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnValue(IMethodSymbol method, string comment, string xmlTag) => AnalyzeStartingPhrase(method, comment, xmlTag, Constants.Comments.ReturnValueStartingPhrase);
    }
}