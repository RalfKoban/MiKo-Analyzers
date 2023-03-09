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

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(IParameterSymbol parameter, string comment, string[] phrase, StringComparison comparison = StringComparison.Ordinal)
        {
            if (comment.StartsWithAny(phrase, comparison) is false)
            {
                var useAllPhrases = phrase.Length > 1 && phrase[0].Length <= 10;
                var proposal = useAllPhrases
                                   ? phrase.HumanizedConcatenated()
                                   : phrase[0].SurroundedWithApostrophe();

                yield return Issue(parameter, string.Intern(proposal));
            }
        }

        protected virtual bool ShallAnalyzeParameter(IParameterSymbol parameter) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var parameter in symbol.Parameters.Where(ShallAnalyzeParameter))
            {
                var parameterComment = parameter.GetComment(commentXml);

                if (parameterComment is null)
                {
                    continue;
                }

                if (parameterComment.EqualsAny(Constants.Comments.UnusedPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var issue in AnalyzeParameter(parameter, parameterComment))
                {
                    yield return issue;
                }
            }
        }
    }
}