using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ArgumentExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        private readonly string[] m_exceptionPhrases;

        protected ArgumentExceptionPhraseAnalyzer(string diagnosticId, Type exceptionType, params string[] phrases) : base(diagnosticId, exceptionType) => m_exceptionPhrases = phrases;

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment)
        {
            switch (symbol)
            {
                case IMethodSymbol method: return AnalyzeException(method, method, exceptionComment);
                case IPropertySymbol property: return AnalyzeException(property, property.SetMethod, exceptionComment);
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeException(ISymbol owningSymbol, IReadOnlyCollection<IParameterSymbol> parameters, string exceptionComment)
        {
            // remove -or- separators and split comment into parts to inspect individually
            var parts = exceptionComment.Split(Constants.Comments.ExceptionSplittingPhrase, StringSplitOptions.RemoveEmptyEntries);

            // create default proposal for parameter names
            var proposal = parameters
                           .Select(_ => string.Format(m_exceptionPhrases[0], _.Name) + Environment.NewLine)
                           .ConcatenatedWith(Constants.Comments.ExceptionSplittingParaPhrase + Environment.NewLine);

            var results = new List<Diagnostic>();

            var parameterIndicators = parameters.ToDictionary(_ => _, _ => string.Format(Constants.Comments.ParamRefBeginningPhrase, _.Name));
            if (exceptionComment.ContainsAny(StringComparison.Ordinal, parameterIndicators.Values.ToArray()))
            {
                foreach (var parameter in parameters)
                {
                    var parameterIndicator = parameterIndicators[parameter];
                    var phrases = m_exceptionPhrases.Select(_ => string.Format(_, parameter.Name)).ToArray();

                    results.AddRange(parts
                                     .Where(_ => _.Contains(parameterIndicator))
                                     .Where(_ => !_.Trim().StartsWithAny(StringComparison.Ordinal, phrases))
                                     .Select(_ => ReportExceptionIssue(owningSymbol, proposal)));
                }
            }
            else
            {
                results.Add(ReportExceptionIssue(owningSymbol, proposal));
            }

            return results;
        }

        protected virtual IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(ImmutableArray<IParameterSymbol> parameterSymbols) => parameterSymbols;

        private IEnumerable<Diagnostic> AnalyzeException(ISymbol owningSymbol, IMethodSymbol methodSymbol, string exceptionComment)
        {
            if (methodSymbol is null) return Enumerable.Empty<Diagnostic>();

            var parameters = GetMatchingParameters(methodSymbol.Parameters);
            return parameters.Any()
                       ? AnalyzeException(owningSymbol, parameters, exceptionComment)
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}