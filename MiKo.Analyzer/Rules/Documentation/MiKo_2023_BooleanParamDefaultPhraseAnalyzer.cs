using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2023_BooleanParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2023";

        public MiKo_2023_BooleanParamDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.RefKind != RefKind.Out && parameter.Type.SpecialType == SpecialType.System_Boolean;

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment)
        {
            var startingPhrase = Constants.Comments.BooleanParameterStartingPhrase;
            var endingPhrase = Constants.Comments.BooleanParameterEndingPhrase;

            return comment.StartsWithAny(StringComparison.Ordinal, startingPhrase) && comment.ContainsAny(StringComparison.Ordinal, endingPhrase)
                ? Enumerable.Empty<Diagnostic>()
                : new[] { ReportIssue(parameter, parameter.Name, startingPhrase[0], endingPhrase[0]) };
        }
    }
}