﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.RefKind != RefKind.Out
                                                                                  && parameter.Type.IsBoolean()
                                                                                  && parameter.GetEnclosingMethod().Name != nameof(IDisposable.Dispose);

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var startingPhrase = Constants.Comments.BooleanParameterStartingPhrase;
            var endingPhrase = Constants.Comments.BooleanParameterEndingPhrase;

            const StringComparison Comparison = StringComparison.Ordinal;

            if (comment.StartsWithAny(startingPhrase, Comparison) is false || comment.ContainsAny(endingPhrase, Comparison) is false)
            {
                yield return Issue(parameter.Name, parameterComment.GetContentsLocation(), startingPhrase[0], endingPhrase[0]);
            }
        }
    }
}