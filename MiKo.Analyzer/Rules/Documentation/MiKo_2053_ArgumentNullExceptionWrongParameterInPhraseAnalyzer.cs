using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2053";

        public MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzer() : base(Id, typeof(ArgumentNullException), false)
        {
        }

        protected override IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(IReadOnlyCollection<IParameterSymbol> parameterSymbols) => parameterSymbols.Where(_ => _.Type.IsValueType && !_.Type.IsNullable()).ToList();

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol owningSymbol, IReadOnlyCollection<IParameterSymbol> parameters, string exceptionComment)
        {
            var parameterIndicators = parameters.ToDictionary(_ => _, _ => string.Format(Constants.Comments.ParamRefBeginningPhrase, _.Name));

            return parameterIndicators
                   .Where(_ => exceptionComment.Contains(_.Value))
                   .Select(_ => ReportIssue(owningSymbol, _.Key.Name, _.Value + Constants.Comments.XmlElementEndingTag))
                   .ToList();
        }
    }
}