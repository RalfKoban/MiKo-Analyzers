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

        public MiKo_2050_ExceptionSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsConstructor())
            {
                return IsParameterlessCtor(symbol)
                    || IsMessageCtor(symbol)
                    || IsMessageExceptionCtor(symbol)
                    || symbol.IsSerializationConstructor();
            }

            return false;
        }

        // overridden because we want to inspect the methods of the type as well
        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsNamespace is false && symbol.IsException())
            {
                // let's see if the type contains a documentation XML
                var typeIssues = base.AnalyzeType(symbol, compilation);

                return typeIssues.Concat(symbol.GetMethods(MethodKind.Constructor).SelectMany(_ => AnalyzeMethod(_, compilation)));
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => base.AnalyzeMethod(symbol, compilation, commentXml, comment).Concat(symbol.Parameters.SelectMany(_ => AnalyzeParameter(_, commentXml))).ToList();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeTypeSummary(type, summaries);
                case IMethodSymbol method: return AnalyzeMethodSummary(method, summaries);
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        private static bool IsParameterlessCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 0;

        private static bool IsMessageCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 1 && IsMessageParameter(symbol.Parameters[0]);

        private static bool IsMessageExceptionCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 2 && IsMessageParameter(symbol.Parameters[0]) && IsExceptionParameter(symbol.Parameters[1]);

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

        private IEnumerable<Diagnostic> AnalyzeTypeSummary(INamedTypeSymbol symbol, IEnumerable<string> summaries) => AnalyzeSummaryPhrase(symbol, summaries, Constants.Comments.ExceptionTypeSummaryStartingPhrase);

        private IEnumerable<Diagnostic> AnalyzeMethodSummary(IMethodSymbol symbol, IEnumerable<string> summaries)
        {
            var defaultPhrases = Constants.Comments.ExceptionCtorSummaryStartingPhrase.Select(_ => _.FormatWith(symbol.ContainingType)).ToArray();

            var findings = AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases);

            if (findings.Any())
            {
                return findings;
            }

            if (IsParameterlessCtor(symbol))
            {
                return AnalyzeOverloadsSummaryPhrase(symbol, defaultPhrases);
            }

            if (IsMessageCtor(symbol))
            {
                return AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases.Select(_ => _ + Constants.Comments.ExceptionCtorMessageParamSummaryContinueingPhrase).ToArray());
            }

            if (IsMessageExceptionCtor(symbol))
            {
                return AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases.Select(_ => _ + Constants.Comments.ExceptionCtorMessageParamSummaryContinueingPhrase + Constants.Comments.ExceptionCtorExceptionParamSummaryContinueingPhrase).ToArray());
            }

            if (symbol.IsSerializationConstructor())
            {
                return Enumerable.Empty<Diagnostic>()
                                 .Concat(AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases.Select(_ => _ + Constants.Comments.ExceptionCtorSerializationParamSummaryContinueingPhrase).ToArray()))
                                 .Concat(AnalyzeRemarksPhrase(symbol, Constants.Comments.ExceptionCtorSerializationParamRemarksPhrase));
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeOverloadsSummaryPhrase(IMethodSymbol symbol, params string[] defaultPhrases)
        {
            var summaries = symbol.GetOverloadSummaries();

            return summaries.Any()
                   ? AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeRemarksPhrase(IMethodSymbol symbol, params string[] defaultPhrases)
        {
            var comments = symbol.GetRemarks();

            return comments.Any()
                   ? AnalyzeStartingPhrase(symbol, Constants.XmlTag.Remarks, comments, defaultPhrases)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, string constant, IEnumerable<string> comments, params string[] phrases)
        {
            if (comments.Any(_ => phrases.Exists(__ => _.StartsWith(__, StringComparison.Ordinal))))
            {
                // fitting comment
            }
            else
            {
                yield return Issue(symbol, constant, phrases[0]);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSummaryPhrase(ISymbol symbol, IEnumerable<string> summaries, params string[] phrases) => AnalyzeStartingPhrase(symbol, Constants.XmlTag.Summary, summaries, phrases);

        private IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, string commentXml)
        {
            var phrases = GetParameterPhrases(symbol);

            return phrases.Length == 0
                   ? Enumerable.Empty<Diagnostic>()
                   : AnalyzeParameter(symbol, commentXml, phrases);
        }

        private IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, string commentXml, IReadOnlyList<string> phrase)
        {
            var comment = symbol.GetComment(commentXml);

            if (phrase.Any(_ => _ == comment))
            {
                // fitting comment
            }
            else
            {
                yield return Issue(symbol, Constants.XmlTag.Param, phrase[0]);
            }
        }
    }
}