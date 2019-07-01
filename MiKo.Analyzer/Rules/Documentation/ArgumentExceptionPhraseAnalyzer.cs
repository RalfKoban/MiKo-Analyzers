using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ArgumentExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        private readonly bool m_addDotsToProposal;
        private readonly string[] m_exceptionPhrases;

        protected ArgumentExceptionPhraseAnalyzer(string diagnosticId, Type exceptionType, bool addDotsToProposal, params string[] phrases) : base(diagnosticId, exceptionType)
        {
            m_addDotsToProposal = addDotsToProposal;
            m_exceptionPhrases = phrases;
        }

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
                           .Select(_ => string.Format(m_exceptionPhrases[0], _.Name) + (m_addDotsToProposal ? "..." : string.Empty) + Environment.NewLine)
                           .ConcatenatedWith(Constants.Comments.ExceptionSplittingParaPhrase + Environment.NewLine);

            var results = new List<Diagnostic>();

            var parameterIndicators = parameters.ToDictionary(_ => _, _ => string.Format(Constants.Comments.ParamRefBeginningPhrase, _.Name));
            var allParameterIndicatorPhrases = parameterIndicators.Values.ToArray();

            const StringComparison Comparison = StringComparison.Ordinal;

            if (exceptionComment.ContainsAny(allParameterIndicatorPhrases, Comparison))
            {
                foreach (var parameter in parameters)
                {
                    var parameterIndicatorPhrase = parameterIndicators[parameter];
                    var phrases = m_exceptionPhrases.Select(_ => string.Format(_, parameter.Name)).ToArray();

                    results.AddRange(parts
                                     .Where(_ => _.Contains(parameterIndicatorPhrase))
                                     .Select(_ => _.Trim())
                                     .Where(_ => _.StartsWithAny(phrases, Comparison) is false)
                                     .Where(_ => _.StartsWithAny(allParameterIndicatorPhrases, Comparison) is false)
                                     .Select(_ => ReportExceptionIssue(owningSymbol, proposal)));
                }
            }
            else
            {
                results.Add(ReportExceptionIssue(owningSymbol, proposal));
            }

            return results;
        }

        protected virtual IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(IReadOnlyCollection<IParameterSymbol> parameterSymbols) => parameterSymbols;

        private IEnumerable<Diagnostic> AnalyzeException(ISymbol owningSymbol, IMethodSymbol methodSymbol, string exceptionComment)
        {
            if (methodSymbol is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var parameters = GetMatchingParameters(methodSymbol.Parameters);

            return parameters.None()
                       ? Enumerable.Empty<Diagnostic>()
                       : AnalyzeException(owningSymbol, parameters, exceptionComment);
        }
    }
}