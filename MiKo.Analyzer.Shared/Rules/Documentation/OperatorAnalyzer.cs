using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
            var commentXml = symbol.GetDocumentationCommentXml();

            var violationsInSummaries = AnalyzeSummaries(comment, method, commentXml);
            var violationsInReturns = AnalyzeReturns(method, commentXml, comment);
            var violationsInParameters = AnalyzeParameters(method.Parameters, commentXml, comment);

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

        private Diagnostic[] AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, string commentXml)
        {
            var summaryXmls = comment.GetSummaryXmls();

            if (summaryXmls.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var summaries = CommentExtensions.GetSummaries(commentXml);

            var phrases = GetSummaryPhrases(symbol);

            if (summaries.None(_ => StringExtensions.EqualsAny(_.AsSpan().Trim(), phrases, StringComparison.OrdinalIgnoreCase)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Summary, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeReturns(ISymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetReturnsPhrases(symbol);
            var comments = CommentExtensions.GetReturns(commentXml);

            if (comments.None(_ => StringExtensions.EqualsAny(_.AsSpan().Trim(), phrases, StringComparison.OrdinalIgnoreCase)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Returns, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeParameters(in ImmutableArray<IParameterSymbol> parameters, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            if (parameters.Length != 2)
            {
                return Array.Empty<Diagnostic>();
            }

            var issue1 = AnalyzeParameter(commentXml, parameters[0], "The first value to compare.");
            var issue2 = AnalyzeParameter(commentXml, parameters[1], "The second value to compare.");

            if (issue1 is null)
            {
                return issue2 is null ? Array.Empty<Diagnostic>() : new[] { issue2 };
            }

            return issue2 is null ? new[] { issue1 } : new[] { issue1, issue2 };
        }

        private Diagnostic AnalyzeParameter(string commentXml, IParameterSymbol parameter, string phrase)
        {
            var comment = parameter.GetComment(commentXml);

            return comment == phrase
                   ? null
                   : Issue(parameter, Constants.XmlTag.Param + " name=\"" + parameter.Name + "\"", phrase);
        }
    }
}