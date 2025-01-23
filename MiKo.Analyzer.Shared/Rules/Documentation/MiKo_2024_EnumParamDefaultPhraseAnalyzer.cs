﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2024_EnumParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2024";

        public MiKo_2024_EnumParamDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.RefKind != RefKind.Out && parameter.Type.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var parameterType = parameter.Type;
            var qualifiedName = parameterType.FullyQualifiedName();

            var phrases = Constants.Comments.EnumParameterStartingPhrase;
            var length = phrases.Length;

            var finalPhrases = new string[2 * length];

            for (var i = 0; i < length; i++)
            {
                // apply full qualified name here as this is applied under the hood to the comment itself
                finalPhrases[i + length] = phrases[i].FormatWith(qualifiedName);

                finalPhrases[i] = phrases[i].FormatWith(parameterType.Name);
            }

            return AnalyzeStartingPhrase(parameter, parameterComment, comment, finalPhrases);
        }
    }
}