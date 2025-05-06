using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParamDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ParamDocumentationAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected bool IgnoreEmptyParameters { get; set; } = true;

        protected override bool ShallAnalyze(ISymbol symbol) => symbol.Kind == SymbolKind.Method;

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol is IMethodSymbol method)
            {
                return AnalyzeParameters(method, comment);
            }

            return Array.Empty<Diagnostic>();
        }

        protected Diagnostic[] AnalyzeStartingPhrase(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment, string[] phrases, in StringComparison comparison = StringComparison.Ordinal)
        {
            if (comment.StartsWithAny(phrases, comparison))
            {
                return Array.Empty<Diagnostic>();
            }

            var phrase = phrases[0];
            var preview = GetPreview(phrase, phrases);

            return new[] { Issue(parameter.Name, GetIssueLocation(parameterComment), preview, CreateProposal(parameter, phrase)) };
        }

        protected Diagnostic[] AnalyzePlainTextStartingPhrase(IParameterSymbol parameter, XmlElementSyntax parameterComment, string[] phrases, in StringComparison comparison = StringComparison.Ordinal)
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

        protected virtual Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment) => Array.Empty<Diagnostic>();

        protected IReadOnlyList<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            var parameters = symbol.Parameters;
            var parametersLength = parameters.Length;

            if (parametersLength <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            var commentXml = symbol.GetDocumentationCommentXml();

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

                    var issues = AnalyzeParameter(parameter, parameterComment, parameterCommentXml);
                    var issuesCount = issues.Length;

                    if (issuesCount > 0)
                    {
                        if (results is null)
                        {
                            results = new List<Diagnostic>(issuesCount);
                        }

                        results.AddRange(issues);
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }

        private static string GetPreview(string phrase, string[] phrases) => phrases.Length > 1 && phrase.Length <= 10
                                                                             ? phrases.HumanizedConcatenated()
                                                                             : phrase.SurroundedWithApostrophe();
    }
}