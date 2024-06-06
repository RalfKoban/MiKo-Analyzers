using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class OperatorAnalyzer : SummaryDocumentationAnalyzer
    {
        private readonly string m_methodName;

        protected OperatorAnalyzer(string diagnosticId, string methodName) : base(diagnosticId, SymbolKind.Method) => m_methodName = methodName;

        protected abstract string[] GetSummaryPhrases(ISymbol symbol);

        protected abstract string[] GetReturnsPhrases(ISymbol symbol);

        protected sealed override bool ShallAnalyze(IMethodSymbol symbol)
        {
            switch (symbol.MethodKind)
            {
                case MethodKind.UserDefinedOperator:
                case MethodKind.Conversion: // that's an unary operator, such as an implicit conversion call
                    return symbol.Name == m_methodName && base.ShallAnalyze(symbol);

                default:
                    return false;
            }
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;

            var violationsInSummaries = AnalyzeSummaries(method, compilation, commentXml, comment);
            var violationsInParameters = AnalyzeParameters(method.Parameters, commentXml, comment);
            var violationsInReturns = AnalyzeReturns(method, commentXml, comment);

            return violationsInSummaries.Concat(violationsInParameters).Concat(violationsInReturns);
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetSummaryPhrases(symbol);

            if (summaries.None(_ => _.AsSpan().Trim().EqualsAny(phrases)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Summary, phrases[0]) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeReturns(ISymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetReturnsPhrases(symbol);
            var comments = CommentExtensions.GetReturns(commentXml);

            if (comments.None(_ => _.AsSpan().Trim().EqualsAny(phrases)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Returns, phrases[0]) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeParameters(ImmutableArray<IParameterSymbol> parameters, string commentXml, DocumentationCommentTriviaSyntax comment)
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