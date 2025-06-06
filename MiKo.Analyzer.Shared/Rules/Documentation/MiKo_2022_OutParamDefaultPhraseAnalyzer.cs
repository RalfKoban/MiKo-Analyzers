﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2022_OutParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2022";

        public MiKo_2022_OutParamDefaultPhraseAnalyzer() : base(Id) => IgnoreEmptyParameters = false;

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.RefKind is RefKind.Out;

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var phrases = GetStartingPhrase(parameter);

            return AnalyzePlainTextStartingPhrase(parameter, parameterComment, phrases);
        }

        protected override Location GetIssueLocation(XmlElementSyntax parameterComment)
        {
            var contents = parameterComment.Content;

            return contents.Count > 0
                   ? GetFirstTextIssueLocation(contents)
                   : base.GetIssueLocation(parameterComment);
        }

        private static string[] GetStartingPhrase(IParameterSymbol parameter) => parameter.Type.IsBoolean()
                                                                                 ? Constants.Comments.OutBoolParameterStartingPhrase
                                                                                 : Constants.Comments.OutParameterStartingPhrase;
    }
}