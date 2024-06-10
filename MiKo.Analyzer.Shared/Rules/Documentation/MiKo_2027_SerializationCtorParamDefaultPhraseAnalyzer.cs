using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameter.IsSerializationInfoParameter())
            {
                return AnalyzeParameterComment(Constants.Comments.CtorSerializationInfoParamPhrase);
            }

            if (parameter.IsStreamingContextParameter())
            {
                return AnalyzeParameterComment(Constants.Comments.CtorStreamingContextParamPhrase);
            }

            return Enumerable.Empty<Diagnostic>();

            IEnumerable<Diagnostic> AnalyzeParameterComment(string[] phrases)
            {
                if (comment.EqualsAny(phrases) is false)
                {
                    var phrase = phrases[0];

                    return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), phrase, CreatePhraseProposal(phrase)) };
                }

                return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}