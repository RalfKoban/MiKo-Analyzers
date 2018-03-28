using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Microsoft.CodeAnalysis;
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

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol)
        {
            if (!symbol.IsConstructor()) return false;

            if (IsParameterlessCtor(symbol)
             || IsMessageCtor(symbol)
             || IsMessageExceptionCtor(symbol)
             || IsSerializationCtor(symbol))
                return !symbol.GetDocumentationCommentXml().IsNullOrWhiteSpace();

            return false; // unknown ctor
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (!symbol.IsException()) return Enumerable.Empty<Diagnostic>();

            return base.AnalyzeType(symbol).Concat(symbol.GetMembers().OfType<IMethodSymbol>().SelectMany(AnalyzeMethod)).ToList();
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => base.AnalyzeMethod(symbol, commentXml).Concat(symbol.Parameters.SelectMany(_ => AnalyzeParameter(_, commentXml))).ToList();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeTypeSummary(type, summaries);
                case IMethodSymbol method: return AnalyzeMethodSummary(method, summaries);
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        private IEnumerable<Diagnostic> AnalyzeTypeSummary(INamedTypeSymbol symbol, IEnumerable<string> summaries) => AnalyzeSummaryPhrase(symbol, summaries, Constants.Comments.ExceptionTypeSummaryStartingPhrase);

        private IEnumerable<Diagnostic> AnalyzeMethodSummary(IMethodSymbol symbol, IEnumerable<string> summaries)
        {
            var defaultPhrases = Constants.Comments.ExceptionCtorSummaryStartingPhrase.Select(_ => string.Format(_, symbol.ContainingType)).ToArray();

            var findings = AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases);
            if (findings.Any()) return findings;

            if (IsParameterlessCtor(symbol))
                return AnalyzeOverloadsSummaryPhrase(symbol, defaultPhrases);

            if (IsMessageCtor(symbol))
                return AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases.Select(_ => _ + Constants.Comments.ExceptionCtorMessageParamSummaryContinueingPhrase).ToArray());

            if (IsMessageExceptionCtor(symbol))
                return AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases.Select(_ => _ + Constants.Comments.ExceptionCtorMessageParamSummaryContinueingPhrase + Constants.Comments.ExceptionCtorExceptionParamSummaryContinueingPhrase).ToArray());

            if (IsSerializationCtor(symbol))
            {
                return AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases.Select(_ => _ + Constants.Comments.ExceptionCtorSerializationParamSummaryContinueingPhrase).ToArray())
                       .Concat(AnalyzeRemarksPhrase(symbol, Constants.Comments.ExceptionCtorSerializationParamRemarksPhrase));
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeOverloadsSummaryPhrase(IMethodSymbol symbol, params string[] defaultPhrases)
        {
            var summaries = GetOverloadSummaries(symbol.GetDocumentationCommentXml());
            return summaries.Any()
                       ? AnalyzeSummaryPhrase(symbol, summaries, defaultPhrases)
                       : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeRemarksPhrase(IMethodSymbol symbol, params string[] defaultPhrases)
        {
            var comments = GetRemarks(symbol.GetDocumentationCommentXml());
            return comments.Any()
                       ? AnalyzeStartingPhrase(symbol, Constants.XmlTag.Remarks, comments, defaultPhrases)
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool IsParameterlessCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 0;

        private static bool IsMessageCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 1 && IsMessageParameter(symbol.Parameters[0]);

        private static bool IsMessageExceptionCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 2 && IsMessageParameter(symbol.Parameters[0]) && IsExceptionParameter(symbol.Parameters[1]);

        private static bool IsSerializationCtor(IMethodSymbol symbol) => symbol.Parameters.Length == 2 && IsSerializationInfoParameter(symbol.Parameters[0]) && IsStreamingContextParameter(symbol.Parameters[1]);

        private static bool IsMessageParameter(IParameterSymbol parameter) => parameter.Type.SpecialType == SpecialType.System_String;

        private static bool IsExceptionParameter(IParameterSymbol parameter) => parameter.Type.IsException();

        private static bool IsSerializationInfoParameter(IParameterSymbol parameter) => parameter.Type.Name == nameof(SerializationInfo);

        private static bool IsStreamingContextParameter(IParameterSymbol parameter) => parameter.Type.Name == nameof(StreamingContext);

        private static string[] GetParameterPhrase(IParameterSymbol symbol)
        {
            if (IsMessageParameter(symbol)) return Constants.Comments.ExceptionCtorMessageParamPhrase;
            if (IsExceptionParameter(symbol)) return Constants.Comments.ExceptionCtorExceptionParamPhrase;
            if (IsSerializationInfoParameter(symbol)) return Constants.Comments.ExceptionCtorSerializationInfoParamPhrase;
            if (IsStreamingContextParameter(symbol)) return Constants.Comments.ExceptionCtorStreamingContextParamPhrase;

            return null;
        }

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, string constant, IEnumerable<string> comments, params string[] phrases) => comments.Any(_ => phrases.Any(__ => _.StartsWith(__, StringComparison.Ordinal)))
                                                                                                                                                ? Enumerable.Empty<Diagnostic>()
                                                                                                                                                : new[] { ReportIssue(symbol, constant, phrases.First()) };

        private IEnumerable<Diagnostic> AnalyzeSummaryPhrase(ISymbol symbol, IEnumerable<string> summaries, params string[] phrases) => AnalyzeStartingPhrase(symbol, Constants.XmlTag.Summary, summaries, phrases);

        private IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, string commentXml)
        {
            var phrase = GetParameterPhrase(symbol);
            return phrase != null
                       ? AnalyzeParameter(symbol, commentXml, phrase)
                       : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, string commentXml, IReadOnlyList<string> phrase)
        {
            var comment = GetParameterComment(symbol, commentXml);
            if (phrase.Any(_ => _ == comment)) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(symbol, Constants.XmlTag.Param, phrase[0]) };
        }
    }
}