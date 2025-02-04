﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2074_ContainsParameterDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2074";

        private static readonly string[] Phrases = { " to seek.", " to locate." };

        public MiKo_2074_ContainsParameterDefaultPhraseAnalyzer() : base(Id) => IgnoreEmptyParameters = false;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase) && symbol.Parameters.Any() && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.ContainingSymbol is IMethodSymbol method && method.Parameters.IndexOf(parameter) == 0;

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameterComment.GetTextTrimmed().EndsWithAny(Phrases, StringComparison.Ordinal))
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), Phrases[0], CreatePhraseProposal(Phrases[0])) };
        }
    }
}