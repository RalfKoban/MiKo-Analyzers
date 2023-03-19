using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParamDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ParamDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, SymbolKind.Method)
        {
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeParameters(symbol, commentXml, comment);

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment, string[] phrase, StringComparison comparison = StringComparison.Ordinal)
        {
            if (comment.StartsWithAny(phrase, comparison) is false)
            {
                var useAllPhrases = phrase.Length > 1 && phrase[0].Length <= 10;
                var proposal = useAllPhrases
                                   ? phrase.HumanizedConcatenated()
                                   : phrase[0].SurroundedWithApostrophe();

                yield return Issue(parameter.Name, parameterComment.GetContentsLocation(), string.Intern(proposal));
            }
        }

        protected IEnumerable<Diagnostic> AnalyzePlainTextStartingPhrase(IParameterSymbol parameter, XmlElementSyntax parameterComment, string[] phrases, StringComparison comparison = StringComparison.Ordinal)
        {
            var text = parameterComment.GetTextTrimmed();

            if (text.StartsWithAny(phrases, comparison))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var useAllPhrases = phrases.Length > 1 && phrases[0].Length <= 10;
            var proposal = useAllPhrases
                               ? phrases.HumanizedConcatenated()
                               : phrases[0].SurroundedWithApostrophe();

            return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), string.Intern(proposal)) };
        }

        protected virtual bool ShallAnalyzeParameter(IParameterSymbol parameter) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var parameter in symbol.Parameters.Where(ShallAnalyzeParameter))
            {
                var parameterComment = comment.GetParameterComment(parameter.Name);

                if (parameterComment is null)
                {
                    continue;
                }

                var parameterCommentXml = parameter.GetComment(commentXml);

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
}