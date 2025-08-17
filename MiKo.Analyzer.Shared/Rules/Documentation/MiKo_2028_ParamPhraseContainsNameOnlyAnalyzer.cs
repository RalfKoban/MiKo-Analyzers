using System;

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

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var phrases = GetPhrases(parameter.Name);

            if (parameterComment.GetTextTrimmed().EqualsAny(phrases, StringComparison.OrdinalIgnoreCase))
            {
                return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation()) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string[] GetPhrases(string parameterName)
        {
            var startingPhrases = Constants.Comments.ParameterStartingPhrase;
            var startingPhrasesLength = startingPhrases.Length;

            var phrases = new string[2 * (startingPhrasesLength + 1)];

            for (var i = 0; i < startingPhrasesLength; i++)
            {
                var phraseWithName = startingPhrases[i] + parameterName;

                phrases[i] = phraseWithName;
                phrases[i + startingPhrasesLength] = phraseWithName + ".";
            }

            phrases[phrases.Length - 2] = parameterName;
            phrases[phrases.Length - 1] = parameterName + ".";

            return phrases;
        }
    }
}