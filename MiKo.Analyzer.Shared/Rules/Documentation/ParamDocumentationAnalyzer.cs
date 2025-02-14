﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParamDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ParamDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, SymbolKind.Method)
        {
        }

        protected bool IgnoreEmptyParameters { get; set; } = true;

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeParameters(symbol, commentXml, comment);

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment, string[] phrases, StringComparison comparison = StringComparison.Ordinal)
        {
            if (comment.StartsWithAny(phrases, comparison))
            {
                return Array.Empty<Diagnostic>();
            }

            var phrase = phrases[0];
            var preview = GetPreview(phrase, phrases);

            return new[] { Issue(parameter.Name, GetIssueLocation(parameterComment), preview, CreateProposal(parameter, phrase)) };
        }

        protected IEnumerable<Diagnostic> AnalyzePlainTextStartingPhrase(IParameterSymbol parameter, XmlElementSyntax parameterComment, string[] phrases, StringComparison comparison = StringComparison.Ordinal)
        {
            var text = parameterComment.GetTextTrimmed();

            if (text.StartsWithAny(phrases, comparison))
            {
                return Array.Empty<Diagnostic>();
            }

            var phrase = phrases[0];
            var preview = GetPreview(phrase, phrases);

            return new[] { Issue(parameter.Name, GetIssueLocation(parameterComment), preview, CreateProposal(parameter, phrase)) };
        }

        protected virtual Location GetIssueLocation(XmlElementSyntax parameterComment) => parameterComment.GetContentsLocation();

        protected virtual Pair[] CreateProposal(IParameterSymbol parameter, string phrase) => CreateStartingPhraseProposal(phrase);

        protected virtual bool ShallAnalyzeParameter(IParameterSymbol parameter) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment) => Array.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var parameters = symbol.Parameters;
            var parametersLength = parameters.Length;

            if (parametersLength <= 0)
            {
                yield break;
            }

            var ignoreEmptyParameters = IgnoreEmptyParameters;

            for (var index = 0; index < parametersLength; index++)
            {
                var parameter = parameters[index];

                if (ShallAnalyzeParameter(parameter))
                {
                    var parameterComment = comment.GetParameterComment(parameter.Name);

                    if (parameterComment is null)
                    {
                        continue;
                    }

                    var parameterCommentXml = parameter.GetComment(commentXml);

                    if (parameterCommentXml.IsNullOrEmpty() && ignoreEmptyParameters)
                    {
                        continue;
                    }

                    if (parameterCommentXml.EqualsAny(Constants.Comments.UnusedPhrase))
                    {
                        continue;
                    }

                    foreach (var issue in AnalyzeParameter(parameter, parameterComment, parameterCommentXml))
                    {
                        yield return issue;
                    }
                }
            }
        }

        private static string GetPreview(string phrase, string[] phrases) => phrases.Length > 1 && phrase.Length <= 10
                                                                             ? phrases.HumanizedConcatenated()
                                                                             : phrase.SurroundedWithApostrophe();
    }
}