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

        protected sealed override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;

            var violationsInSummaries = AnalyzeSummary(method, comment);
            var violationsInParameters = AnalyzeParameters(method.Parameters, comment);
            var violationsInReturns = AnalyzeReturns(method, comment);

            return violationsInSummaries.Concat(violationsInParameters).Concat(violationsInReturns);
        }

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml)
        {
            var phrases = GetSummaryPhrases(symbol);

            // TODO RKN: fix summary
            var summary = summaryXml.ToString().Trim();
            if (summary.EqualsAny(phrases))
            {
                return null;
            }

            return Issue(symbol, Constants.XmlTag.Summary, phrases[0]);
        }

        private IEnumerable<Diagnostic> AnalyzeReturns(ISymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetReturnsPhrases(symbol);

            // TODO RKN: fix returns
            var comments = comment.GetReturnsXmls();
            if (comments.Any(_ => _.ToString().Trim().EqualsAny(phrases)))
            {
                yield break;
            }

            yield return Issue(symbol, Constants.XmlTag.Returns, phrases[0]);
        }

        private IEnumerable<Diagnostic> AnalyzeParameters(ImmutableArray<IParameterSymbol> parameters, DocumentationCommentTriviaSyntax comment)
        {
            if (parameters.Length == 2)
            {
                yield return AnalyzeParameter(comment, parameters[0], "The first value to compare.");
                yield return AnalyzeParameter(comment, parameters[1], "The second value to compare.");
            }
        }

        private Diagnostic AnalyzeParameter(DocumentationCommentTriviaSyntax comment, IParameterSymbol parameter, string phrase)
        {
            var parameterComment = comment.GetParameterComment(parameter.Name);

            var hasPhrase = parameterComment.DescendantNodes<XmlTextSyntax>()
                                            .SelectMany(_ => _.TextTokens)
                                            .Any(_ => _.ValueText == phrase);

            return hasPhrase
                       ? null
                       : Issue(parameter.Name, parameterComment, $"{Constants.XmlTag.Param} name=\"{parameter.Name}\"", phrase);
        }
    }
}