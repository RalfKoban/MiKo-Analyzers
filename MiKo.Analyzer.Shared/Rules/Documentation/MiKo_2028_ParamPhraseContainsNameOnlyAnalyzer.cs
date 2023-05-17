using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2028_ParamPhraseContainsNameOnlyAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2028";

        public MiKo_2028_ParamPhraseContainsNameOnlyAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var phrases = GetPhrases(parameter.Name);

            if (parameterComment.GetTextTrimmed().EqualsAny(phrases))
            {
                yield return Issue(parameter.Name, parameterComment.GetContentsLocation());
            }
        }

        private static string[] GetPhrases(string parameterName)
        {
            var startingPhrases = Constants.Comments.ParameterStartingPhrase;

            var phrases = new string[2 * (startingPhrases.Length + 1)];

            for (var i = 0; i < startingPhrases.Length; i++)
            {
                var phraseWithName = startingPhrases[i] + parameterName;

                phrases[i] = phraseWithName;
                phrases[i + startingPhrases.Length] = phraseWithName + ".";
            }

            phrases[phrases.Length - 2] = parameterName;
            phrases[phrases.Length - 1] = parameterName + ".";

            return phrases;
        }
    }
}