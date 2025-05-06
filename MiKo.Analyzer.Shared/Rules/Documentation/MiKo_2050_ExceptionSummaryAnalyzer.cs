using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2050_ExceptionSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2050";

        public MiKo_2050_ExceptionSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type:
                {
                    return type.IsNamespace is false && type.IsException();
                }

                case IMethodSymbol method when method.IsConstructor() && method.ContainingType.IsException():
                {
                    return IsParameterlessCtor(method)
                        || IsMessageCtor(method)
                        || IsMessageExceptionCtor(method)
                        || method.IsSerializationConstructor();
                }

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<IReadOnlyCollection<string>> summaries)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type:
                    return AnalyzeTypeSummary(type, summaries.Value, comment);

                case IMethodSymbol method:
                {
                    var parameters = method.Parameters;

                    var issues = new List<Diagnostic>(AnalyzeMethodSummary(method, summaries.Value, comment));

                    if (parameters.Length > 0)
                    {
                        issues.AddRange(parameters.SelectMany(_ => AnalyzeParameter(_, commentXml.Value, comment)));
                    }

                    return issues;
                }

                default:
                    return Array.Empty<Diagnostic>();
            }
        }

        private static bool IsParameterlessCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 0;

        private static bool IsMessageCtor(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            return parameters.Length == 1 && IsMessageParameter(parameters[0]);
        }

        private static bool IsMessageExceptionCtor(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            return parameters.Length == 2 && IsMessageParameter(parameters[0]) && IsExceptionParameter(parameters[1]);
        }

        private static bool IsMessageParameter(IParameterSymbol parameter) => parameter.Type.IsString();

        private static bool IsExceptionParameter(IParameterSymbol parameter) => parameter.Type.IsException();

        private static string[] GetParameterPhrases(IParameterSymbol symbol)
        {
            if (IsMessageParameter(symbol))
            {
                return Constants.Comments.ExceptionCtorMessageParamPhrase;
            }

            if (IsExceptionParameter(symbol))
            {
                return Constants.Comments.ExceptionCtorExceptionParamPhrase;
            }

            if (symbol.IsSerializationInfoParameter())
            {
                return Constants.Comments.CtorSerializationInfoParamPhrase;
            }

            if (symbol.IsStreamingContextParameter())
            {
                return Constants.Comments.CtorStreamingContextParamPhrase;
            }

            return Array.Empty<string>();
        }

        private Diagnostic[] AnalyzeTypeSummary(INamedTypeSymbol symbol, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment) => AnalyzeSummaryPhrase(symbol, summaries, comment, Constants.Comments.ExceptionTypeSummaryStartingPhrase);

        private IReadOnlyList<Diagnostic> AnalyzeMethodSummary(IMethodSymbol symbol, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            var defaultPhrases = Constants.Comments.ExceptionCtorSummaryStartingPhrase.ToArray(_ => _.FormatWith(symbol.ContainingType));

            var findings = AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases);

            if (findings.Length > 0)
            {
                return findings;
            }

            if (IsParameterlessCtor(symbol))
            {
                return AnalyzeOverloadsSummaryPhrase(symbol, comment, defaultPhrases);
            }

            if (IsMessageCtor(symbol))
            {
                return AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases.ToArray(_ => _ + Constants.Comments.ExceptionCtorMessageParamSummaryContinuingPhrase));
            }

            if (IsMessageExceptionCtor(symbol))
            {
                return AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases.ToArray(_ => _ + Constants.Comments.ExceptionCtorMessageParamSummaryContinuingPhrase + Constants.Comments.ExceptionCtorExceptionParamSummaryContinuingPhrase));
            }

            if (symbol.IsSerializationConstructor())
            {
                var issues = new List<Diagnostic>();

                issues.AddRange(AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases.ToArray(_ => _ + Constants.Comments.ExceptionCtorSerializationParamSummaryContinuingPhrase)));
                issues.AddRange(AnalyzeRemarksPhrase(symbol, comment, Constants.Comments.ExceptionCtorSerializationParamRemarksPhrase));

                return issues;
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeOverloadsSummaryPhrase(IMethodSymbol symbol, DocumentationCommentTriviaSyntax comment, params string[] defaultPhrases)
        {
            var summaries = symbol.GetOverloadSummaries();

            if (summaries.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases);
        }

        private Diagnostic[] AnalyzeRemarksPhrase(IMethodSymbol symbol, DocumentationCommentTriviaSyntax comment, params string[] defaultPhrases)
        {
            var comments = symbol.GetRemarks();

            if (comments.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return AnalyzeStartingPhrase(symbol, Constants.XmlTag.Remarks, comments, comment, defaultPhrases);
        }

        private Diagnostic[] AnalyzeStartingPhrase(ISymbol symbol, string xmlTag, IEnumerable<string> comments, DocumentationCommentTriviaSyntax comment, params string[] phrases)
        {
            if (comments.None(_ => phrases.Exists(__ => _.StartsWith(__, StringComparison.Ordinal))))
            {
                // find XML
                var problematicComments = comment.GetXmlSyntax(xmlTag);

                var issue = problematicComments.Count == 0
                            ? Issue(symbol, xmlTag, phrases[0])
                            : Issue(symbol.Name, problematicComments[0], xmlTag, phrases[0]);

                return new[] { issue };
            }

            // fitting comment
            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeSummaryPhrase(ISymbol symbol, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment, params string[] phrases) => AnalyzeStartingPhrase(symbol, Constants.XmlTag.Summary, summaries, comment, phrases);

        private Diagnostic[] AnalyzeParameter(IParameterSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetParameterPhrases(symbol);

            return phrases.Length == 0
                   ? Array.Empty<Diagnostic>()
                   : AnalyzeParameter(symbol, commentXml, comment, phrases);
        }

        private Diagnostic[] AnalyzeParameter(IParameterSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment, in ReadOnlySpan<string> phrases)
        {
            var parameterCommentXml = symbol.GetComment(commentXml);

            if (phrases.None(_ => _ == parameterCommentXml))
            {
                var parameterComment = comment.GetParameterComment(symbol.Name);

                var issue = parameterComment is null
                            ? Issue(symbol, Constants.XmlTag.Param, phrases[0])
                            : Issue(symbol.Name, parameterComment, Constants.XmlTag.Param, phrases[0]);

                return new[] { issue };
            }

            // fitting comment
            return Array.Empty<Diagnostic>();
        }
    }
}