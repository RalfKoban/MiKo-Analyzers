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

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return base.AnalyzeMethod(symbol, compilation, commentXml, comment)
                       .Concat(symbol.Parameters.SelectMany(_ => AnalyzeParameter(_, commentXml, comment)))
                       .ToList();
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeTypeSummary(type, summaries, comment);
                case IMethodSymbol method: return AnalyzeMethodSummary(method, summaries, comment);
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

        private IEnumerable<Diagnostic> AnalyzeTypeSummary(INamedTypeSymbol symbol, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment) => AnalyzeSummaryPhrase(symbol, summaries, comment, Constants.Comments.ExceptionTypeSummaryStartingPhrase);

        private IEnumerable<Diagnostic> AnalyzeMethodSummary(IMethodSymbol symbol, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            var defaultPhrases = Constants.Comments.ExceptionCtorSummaryStartingPhrase.ToArray(_ => _.FormatWith(symbol.ContainingType));

            var findings = AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases);

            if (findings.Any())
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
                return Enumerable.Empty<Diagnostic>()
                                 .Concat(AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases.ToArray(_ => _ + Constants.Comments.ExceptionCtorSerializationParamSummaryContinuingPhrase)))
                                 .Concat(AnalyzeRemarksPhrase(symbol, comment, Constants.Comments.ExceptionCtorSerializationParamRemarksPhrase));
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeOverloadsSummaryPhrase(IMethodSymbol symbol, DocumentationCommentTriviaSyntax comment, params string[] defaultPhrases)
        {
            var summaries = symbol.GetOverloadSummaries();

            return summaries.Any()
                   ? AnalyzeSummaryPhrase(symbol, summaries, comment, defaultPhrases)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeRemarksPhrase(IMethodSymbol symbol, DocumentationCommentTriviaSyntax comment, params string[] defaultPhrases)
        {
            var comments = symbol.GetRemarks();

            return comments.Any()
                   ? AnalyzeStartingPhrase(symbol, Constants.XmlTag.Remarks, comments, comment, defaultPhrases)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, string xmlTag, IEnumerable<string> comments, DocumentationCommentTriviaSyntax comment, params string[] phrases)
        {
            if (comments.None(_ => phrases.Exists(__ => _.StartsWith(__, StringComparison.Ordinal))))
            {
                // find XML
                var problematicComment = comment.GetXmlSyntax(xmlTag).FirstOrDefault();

                var issue = problematicComment is null
                            ? Issue(symbol, xmlTag, phrases[0])
                            : Issue(symbol.Name, problematicComment, xmlTag, phrases[0]);

                return new[] { issue };
            }

            // fitting comment
            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeSummaryPhrase(ISymbol symbol, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment, params string[] phrases) => AnalyzeStartingPhrase(symbol, Constants.XmlTag.Summary, summaries, comment, phrases);

        private IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var phrases = GetParameterPhrases(symbol);

            return phrases.Length == 0
                   ? Enumerable.Empty<Diagnostic>()
                   : AnalyzeParameter(symbol, commentXml, comment, phrases);
        }

        private IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment, IReadOnlyList<string> phrase)
        {
            var parameterCommentXml = symbol.GetComment(commentXml);

            if (phrase.None(_ => _ == parameterCommentXml))
            {
                var parameterComment = comment.GetParameterComment(symbol.Name);

                var issue = parameterComment is null
                            ? Issue(symbol, Constants.XmlTag.Param, phrase[0])
                            : Issue(symbol.Name, parameterComment, Constants.XmlTag.Param, phrase[0]);

                return new[] { issue };
            }

            // fitting comment
            return Enumerable.Empty<Diagnostic>();
        }
    }
}