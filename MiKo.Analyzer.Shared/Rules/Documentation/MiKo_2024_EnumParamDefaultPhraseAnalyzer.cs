using System;

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

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var phrases = parameter.HasFlags()
                          ? Constants.Comments.EnumFlagsParameterStartingPhrase
                          : Constants.Comments.EnumParameterStartingPhrase;

            var adjustedPhrases = AdjustPhrases(parameter, phrases);

            return AnalyzeStartingPhrase(parameter, parameterComment, comment, adjustedPhrases);
        }

        protected override Pair[] CreateProposal(IParameterSymbol parameter, string phrase)
        {
            return new[]
                       {
                           new Pair(Constants.AnalyzerCodeFixSharedData.StartingPhrase, phrase),
                           new Pair(Constants.AnalyzerCodeFixSharedData.IsFlagged, parameter.HasFlags().ToString()),
                       };
        }

        private static string[] AdjustPhrases(IParameterSymbol parameter, in ReadOnlySpan<string> phrases)
        {
            var parameterType = parameter.Type;
            var qualifiedName = parameterType.FullyQualifiedName();
            var parameterTypeName = parameterType.Name;

            var length = phrases.Length;

            var finalPhrases = new string[2 * length];

            for (var i = 0; i < length; i++)
            {
                var phrase = phrases[i];

                // apply full qualified name here as this is applied under the hood to the comment itself
                finalPhrases[i + length] = phrase.FormatWith(qualifiedName);
                finalPhrases[i] = phrase.FormatWith(parameterTypeName);
            }

            return finalPhrases;
        }
    }
}