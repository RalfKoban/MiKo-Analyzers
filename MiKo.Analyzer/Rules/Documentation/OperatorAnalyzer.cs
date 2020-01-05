using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class OperatorAnalyzer : SummaryDocumentationAnalyzer
    {
        private readonly string m_methodName;

        protected OperatorAnalyzer(string diagnosticId, string methodName) : base(diagnosticId, SymbolKind.Method) => m_methodName = methodName;

        protected abstract string[] GetSummaryPhrases(ISymbol symbol);

        protected abstract string[] GetReturnsPhrases(ISymbol symbol);

        protected sealed override bool ShallAnalyzeMethod(IMethodSymbol symbol)
        {
            switch (symbol.MethodKind)
            {
                case MethodKind.UserDefinedOperator:
                case MethodKind.Conversion: // that's an unary operator, such as an implicit conversion call
                    return symbol.Name == m_methodName;

                default:
                    return false;
            }
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var method = (IMethodSymbol)symbol;

            var violationsInSummaries = AnalyzeSummaries(method, commentXml);
            var violationsInParameters = AnalyzeParameters(method.Parameters, commentXml);
            var violationsInReturns = AnalyzeReturns(method, commentXml);

            return violationsInSummaries.Concat(violationsInParameters).Concat(violationsInReturns).Where(_ => _ != null);
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = GetSummaryPhrases(symbol);

            return summaries.Any(_ => _.Trim().EqualsAny(phrases))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, Constants.XmlTag.Summary, phrases[0]) };
        }

        private IEnumerable<Diagnostic> AnalyzeReturns(ISymbol symbol, string commentXml)
        {
            var phrases = GetReturnsPhrases(symbol);
            var comments = CommentExtensions.GetReturns(commentXml);

            return comments.Any(_ => _.Trim().EqualsAny(phrases))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, Constants.XmlTag.Returns, phrases[0]) };
        }

        private IEnumerable<Diagnostic> AnalyzeParameters(ImmutableArray<IParameterSymbol> parameters, string commentXml)
        {
            if (parameters.Length == 2)
            {
                yield return AnalyzeParameter(commentXml, parameters[0], "The first value to compare.");
                yield return AnalyzeParameter(commentXml, parameters[1], "The second value to compare.");
            }
        }

        private Diagnostic AnalyzeParameter(string commentXml, IParameterSymbol parameter, string phrase)
        {
            var comment = parameter.GetComment(commentXml);
            return comment == phrase
                       ? null
                       : Issue(parameter, $"{Constants.XmlTag.Param} name=\"{parameter.Name}\"", phrase);
        }
    }
}