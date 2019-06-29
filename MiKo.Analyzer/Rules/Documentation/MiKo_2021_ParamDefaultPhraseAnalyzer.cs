using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2021_ParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2021";

        public MiKo_2021_ParamDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter)
        {
            if (parameter.RefKind == RefKind.Out)
            {
                return false; // MiKo 2022
            }

            if (parameter.Type.IsEnum())
            {
                return false; // MiKo 2023
            }

            if (parameter.Type.IsCancellationToken())
            {
                return false; // MiKo 2024
            }

            return parameter.Type.SpecialType != SpecialType.System_Boolean;
        }

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment) => AnalyzeStartingPhrase(parameter, comment, Constants.Comments.ParameterStartingPhrase);
    }
}