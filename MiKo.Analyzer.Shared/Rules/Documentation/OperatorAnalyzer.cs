using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class OperatorAnalyzer : DocumentationAnalyzer
    {
        private readonly string m_methodName;

        protected OperatorAnalyzer(string diagnosticId, string methodName) : base(diagnosticId) => m_methodName = methodName;

        protected abstract string[] GetSummaryPhrases(ISymbol symbol);

        protected abstract string[] GetReturnsPhrases(ISymbol symbol);

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            if (symbol is IMethodSymbol method)
            {
                switch (method.MethodKind)
                {
                    case MethodKind.UserDefinedOperator:
                    case MethodKind.Conversion: // that's a unary operator, such as an implicit conversion call
                        return method.Name == m_methodName;

                    default:
                        return false;
                }
            }

            return false;
        }

        protected sealed override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var method = (IMethodSymbol)symbol;

            var violationsInSummaries = AnalyzeSummaries(comment, method);
            var violationsInReturns = AnalyzeReturns(comment, method);
            var violationsInParameters = AnalyzeParameters(comment, method.Parameters);

            var issueCount = violationsInSummaries.Length + violationsInReturns.Length + violationsInParameters.Length;

            if (issueCount is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var results = new List<Diagnostic>(issueCount);
            results.AddRange(violationsInSummaries);
            results.AddRange(violationsInReturns);
            results.AddRange(violationsInParameters);

            return results;
        }

        private Diagnostic[] AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            var summaryXmls = comment.GetSummaryXmls();

            if (summaryXmls.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var phrases = GetSummaryPhrases(symbol);

            if (summaryXmls.None(_ => _.GetTextTrimmed().EqualsAny(phrases, StringComparison.OrdinalIgnoreCase)))
            {
                return new[] { Issue(symbol.Name, summaryXmls[0].GetContentsLocation(), Constants.XmlTag.Summary, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeReturns(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            var returnXmls = comment.GetReturnsXmls();

            if (returnXmls.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var phrases = GetReturnsPhrases(symbol);

            if (returnXmls.None(_ => _.GetTextTrimmed().EqualsAny(phrases, StringComparison.OrdinalIgnoreCase)))
            {
                return new[] { Issue(symbol.Name, returnXmls[0].GetContentsLocation(), Constants.XmlTag.Returns, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeParameters(DocumentationCommentTriviaSyntax comment, in ImmutableArray<IParameterSymbol> parameters)
        {
            if (parameters.Length != 2)
            {
                return Array.Empty<Diagnostic>();
            }

            var issue1 = AnalyzeParameter(comment, parameters[0], "The first value to compare.");
            var issue2 = AnalyzeParameter(comment, parameters[1], "The second value to compare.");

            if (issue1 is null)
            {
                return issue2 is null ? Array.Empty<Diagnostic>() : new[] { issue2 };
            }

            return issue2 is null ? new[] { issue1 } : new[] { issue1, issue2 };
        }

        private Diagnostic AnalyzeParameter(DocumentationCommentTriviaSyntax comment, IParameterSymbol parameter, string phrase)
        {
            var parameterName = parameter.Name;
            string paramName = Constants.XmlTag.Param + " name=\"" + parameterName + "\"";

            var parameterComment = comment.GetParameterComment(parameterName);

            if (parameterComment is null)
            {
                return Issue(parameter, paramName, phrase);
            }

            var text = parameterComment.GetTextTrimmed();

            if (text == phrase)
            {
                return null;
            }

            return Issue(parameterName, parameterComment.GetContentsLocation(), paramName, phrase);
        }
    }
}