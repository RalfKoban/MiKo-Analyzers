using System;
using System.Collections.Generic;

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
            foreach (var parameter in symbol.Parameters)
            {
                var comment = parameter.GetComment(commentXml);

                if (comment.ContainsAny(Constants.Comments.FuturePhrase))
                {
                    yield return Issue(parameter);
                }
            }
        }
    }
}