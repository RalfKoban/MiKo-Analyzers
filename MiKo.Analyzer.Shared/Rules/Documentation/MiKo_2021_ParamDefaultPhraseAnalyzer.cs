﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            if (parameter.RefKind is RefKind.Out)
            {
                return false; // MiKo 2022
            }

            var parameterType = parameter.Type;

            if (parameterType.IsEnum())
            {
                return false; // MiKo 2023
            }

            if (parameterType.IsCancellationToken())
            {
                return false; // MiKo 2024
            }

            return parameterType.IsBoolean() is false;
        }

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            return AnalyzePlainTextStartingPhrase(parameter, parameterComment, Constants.Comments.ParameterStartingPhrase, StringComparison.OrdinalIgnoreCase);
        }

        protected override Location GetIssueLocation(XmlElementSyntax parameterComment)
        {
            var content = parameterComment.Content;

            return content.Count > 0
                   ? GetFirstTextIssueLocation(content)
                   : base.GetIssueLocation(parameterComment);
        }
    }
}