using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2025";

        public MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.RefKind != RefKind.Out && parameter.Type.IsCancellationToken();

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment) => AnalyzePlainTextStartingPhrase(parameter, parameterComment, Constants.Comments.CancellationTokenParameterPhrase);
    }
}