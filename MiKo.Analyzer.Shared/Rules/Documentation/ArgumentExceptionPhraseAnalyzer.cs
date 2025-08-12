using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ArgumentExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        private readonly bool m_addDotsToProposal;
        private readonly string[] m_exceptionPhrases;

        protected ArgumentExceptionPhraseAnalyzer(string diagnosticId, Type exceptionType, in bool addDotsToProposal, params string[] phrases) : base(diagnosticId, exceptionType)
        {
            m_addDotsToProposal = addDotsToProposal;
            m_exceptionPhrases = phrases;
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment)
        {
            switch (symbol)
            {
                case IMethodSymbol method: return AnalyzeException(method, method, exceptionComment);
                case IPropertySymbol property: return AnalyzeException(property, property.SetMethod, exceptionComment);
                default: return Array.Empty<Diagnostic>();
            }
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeException(ISymbol owningSymbol, IReadOnlyCollection<IParameterSymbol> parameters, XmlElementSyntax exceptionComment)
        {
            // get rid of the para tags as we are not interested into them
            var comment = exceptionComment.GetTextTrimmed();

            // remove -or- separators and split comment into parts to inspect individually
            var parts = comment.SplitBy(Constants.Comments.ExceptionSplittingPhrase);

            // create default proposal for parameter names
            var proposal = parameters.Select(_ => m_exceptionPhrases[0].FormatWith(_.Name) + (m_addDotsToProposal ? "..." + Constants.EnvironmentNewLine : string.Empty + Constants.EnvironmentNewLine))
                                     .ConcatenatedWith(Constants.Comments.ExceptionSplittingParaPhrase + Constants.EnvironmentNewLine);

            var parameterIndicators = parameters.ToDictionary<IParameterSymbol, IParameterSymbol, string>(_ => _, _ => Constants.Comments.ParamRefBeginningPhrase.FormatWith(_.Name), SymbolEqualityComparer.Default);
            var allParameterIndicatorPhrases = parameterIndicators.Values.ToArray();

            var results = new List<Diagnostic>(1);

            if (comment.ContainsAny(allParameterIndicatorPhrases))
            {
                foreach (var parameter in parameters)
                {
                    var parameterIndicatorPhrase = parameterIndicators[parameter];
                    var phrases = m_exceptionPhrases.ToArray(_ => _.FormatWith(parameter.Name));

                    foreach (var part in parts)
                    {
                        if (part.Contains(parameterIndicatorPhrase))
                        {
                            var trimmed = part.AsSpan().Trim();

                            if (trimmed.StartsWithAny(phrases) is false && trimmed.StartsWithAny(allParameterIndicatorPhrases) is false)
                            {
                                results.Add(ExceptionIssue(exceptionComment, proposal));
                            }
                        }
                    }
                }
            }
            else
            {
                results.Add(ExceptionIssue(exceptionComment, proposal));
            }

            return results;
        }

        protected virtual IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(in ImmutableArray<IParameterSymbol> parameterSymbols) => parameterSymbols;

        private IReadOnlyList<Diagnostic> AnalyzeException(ISymbol owningSymbol, IMethodSymbol methodSymbol, XmlElementSyntax exceptionComment)
        {
            if (methodSymbol is null)
            {
                return Array.Empty<Diagnostic>();
            }

            var parameters = GetMatchingParameters(methodSymbol.Parameters);

            return parameters.Count is 0
                   ? Array.Empty<Diagnostic>()
                   : AnalyzeException(owningSymbol, parameters, exceptionComment);
        }
    }
}