using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3027_FutureUsedParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3027";

        public MiKo_3027_FutureUsedParameterAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Parameters.Length > 0;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var commentXml = symbol.GetComment();

            if (commentXml.IsNullOrEmpty())
            {
                return Array.Empty<Diagnostic>();
            }

            return Analyze(symbol.Parameters, commentXml);
        }

        private IEnumerable<Diagnostic> Analyze(ImmutableArray<IParameterSymbol> parameters, string commentXml)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, parametersLength = parameters.Length; index < parametersLength; index++)
            {
                var parameter = parameters[index];
                var comment = parameter.GetComment(commentXml);

                if (comment.IsNullOrEmpty())
                {
                    continue;
                }

                if (comment.ContainsAny(Constants.Comments.FuturePhrase))
                {
                    yield return Issue(parameter);
                }
            }
        }
    }
}