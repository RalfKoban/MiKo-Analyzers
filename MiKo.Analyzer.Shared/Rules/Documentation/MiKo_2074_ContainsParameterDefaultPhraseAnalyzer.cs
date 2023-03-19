using System;
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

        internal const string Phrase = " to seek.";

        private static readonly string[] Phrases = { Phrase, " to locate." };

        public MiKo_2074_ContainsParameterDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase) && symbol.Parameters.Any() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameterComment.GetTextTrimmed().EndsWithAny(Phrases, StringComparison.Ordinal) is false)
            {
                yield return Issue(parameter.Name, parameterComment.StartTag, Phrase);
            }
        }
    }
}