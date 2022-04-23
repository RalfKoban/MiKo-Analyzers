using System.Collections.Generic;

using Microsoft.CodeAnalysis;
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

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment)
        {
            var phrases = Constants.Comments.EnumParameterStartingPhrase;
            var values = new List<string>(phrases.Length);

            foreach (var phrase in phrases)
            {
                if (phrase.Contains("{0}"))
                {
                    // apply full qualified name here as this is applied under the hood to the comment itself
                    values.Add(string.Format(phrase, parameter.Type.FullyQualifiedName()));
                    values.Add(string.Format(phrase, parameter.Type.Name));
                }
                else
                {
                    values.Add(phrase);
                }
            }

            return AnalyzeStartingPhrase(parameter, comment, values.ToArray());
        }
    }
}