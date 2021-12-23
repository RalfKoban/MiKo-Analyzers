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
            if (comment.StartsWithAny(phrase, StringComparison.Ordinal))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var useAllPhrases = phrase.Length > 1 && phrase[0].Length <= 10;
            var proposal = useAllPhrases
                               ? phrase.HumanizedConcatenated()
                               : phrase[0].SurroundedWithApostrophe();

            return new[] { Issue(parameter, string.Intern(proposal)) };
        }

        protected virtual bool ShallAnalyzeParameter(IParameterSymbol parameter) => true;

        protected virtual IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, string comment) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;
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

                AnalyzeParameters(parameter, comment, ref results);
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private void AnalyzeParameters(IParameterSymbol parameter, string comment, ref List<Diagnostic> results)
        {
            var findings = AnalyzeParameter(parameter, comment);
            if (findings.Any())
            {
                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.AddRange(findings);
            }
        }
    }
}