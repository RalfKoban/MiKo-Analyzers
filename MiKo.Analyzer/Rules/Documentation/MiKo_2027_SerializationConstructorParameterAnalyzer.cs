using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2027_SerializationConstructorParameterAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2027";

        public MiKo_2027_SerializationConstructorParameterAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.IsSerializationConstructor();

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.IsSerializationInfoParameter() || parameter.IsStreamingContextParameter();

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment)
        {
            if (parameter.IsSerializationInfoParameter())
            {
                var phrases = Constants.Comments.CtorSerializationInfoParamPhrase;

                if (!comment.EqualsAny(phrases))
                    return new[] { Issue(parameter, phrases[0]) };
            }
            else if (parameter.IsStreamingContextParameter())
            {
                var phrases = Constants.Comments.CtorStreamingContextParamPhrase;

                if (!comment.EqualsAny(phrases))
                    return new[] { Issue(parameter, phrases[0]) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}