using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2051_ArgumentNullExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2051";

        public MiKo_2051_ArgumentNullExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentNullException))
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment)
        {
            List<Diagnostic> results = null;

            if (symbol is IMethodSymbol method && method.Parameters.Any())
            {
                // remove -or- separators and split comment into parts to inspect individually
                var parts = exceptionComment.Split(Constants.Comments.ExceptionSplittingPhrase, StringSplitOptions.RemoveEmptyEntries);

                foreach (var parameter in method.Parameters.Where(_ => _.Type.TypeKind != TypeKind.Struct))
                {
                    var parameterIndicator = string.Format(Constants.Comments.ParamRefBeginningPhrase, parameter.Name);
                    var phrases = Constants.Comments.ArgumentNullExceptionStartingPhrase.Select(_ => string.Format(_, parameter.Name)).ToArray();

                    if (parts.Where(_ => _.Contains(parameterIndicator))
                             .Select(_ => _.Trim()) // get rid of white-spaces
                             .Any(_ => _.EqualsAny(StringComparison.Ordinal, phrases)))
                        continue;

                    if (results == null) results = new List<Diagnostic>();
                    results.Add(ReportIssue(parameter, ExceptionPhrase, phrases[0]));
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}