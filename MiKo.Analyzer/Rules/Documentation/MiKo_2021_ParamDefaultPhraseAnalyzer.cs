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

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => AnalyzeParameters(symbol, commentXml);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter)
        {
            if (parameter.RefKind == RefKind.Out) return false;
            if (parameter.Type.IsEnum()) return false;

            // ReSharper disable once RedundantNameQualifier
            if (parameter.Type.Name == nameof(System.Boolean)) return false;

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment) => AnalyzeStartingPhrase(parameter, comment, Constants.Comments.ParameterStartingPhrase);
    }
}