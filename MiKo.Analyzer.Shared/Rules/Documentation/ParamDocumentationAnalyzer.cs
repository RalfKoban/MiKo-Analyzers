using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParamDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ParamDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, SymbolKind.Method)
        {
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml) => AnalyzeParameters(symbol, commentXml);

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(IParameterSymbol parameter, string comment, string[] phrase)
        {
            if (comment.StartsWithAny(phrase, StringComparison.Ordinal) is false)
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

        protected IEnumerable<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, string commentXml)
        {
            foreach (var parameter in symbol.Parameters.Where(ShallAnalyzeParameter))
            {
                var comment = parameter.GetComment(commentXml);
                if (comment is null)
                {
                    continue;
                }

                if (comment.EqualsAny(Constants.Comments.UnusedPhrase, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var issue in AnalyzeParameter(parameter, comment))
                {
                    yield return issue;
                }
            }
        }
    }
}