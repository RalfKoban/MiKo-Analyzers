using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2027_SerializationCtorParamDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2027";

        public MiKo_2027_SerializationCtorParamDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsSerializationConstructor() && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.IsSerializationInfoParameter() || parameter.IsStreamingContextParameter();

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameter.IsSerializationInfoParameter())
            {
                return AnalyzeParameterComment(parameter.Name, parameterComment, comment, Constants.Comments.CtorSerializationInfoParamPhrase);
            }

            if (parameter.IsStreamingContextParameter())
            {
                return AnalyzeParameterComment(parameter.Name, parameterComment, comment, Constants.Comments.CtorStreamingContextParamPhrase);
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeParameterComment(string parameterName, XmlElementSyntax parameterComment, string comment, string[] phrases)
        {
            if (comment.EqualsAny(phrases))
            {
                return Array.Empty<Diagnostic>();
            }

            var phrase = phrases[0];

            return new[] { Issue(parameterName, parameterComment.GetContentsLocation(), phrase, CreatePhraseProposal(phrase)) };
        }
    }
}