﻿using System;
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

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeParameters(symbol, comment);

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

        protected IEnumerable<Diagnostic> AnalyzeParameters(IMethodSymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var parameter in symbol.Parameters.Where(ShallAnalyzeParameter))
            {
                var parameterComment = comment.GetParameterComment(parameter.Name);

                if (parameterComment is null)
                {
                    continue;
                }

                // TODO RKN: Use parameter comment instead
                var text = parameterComment.GetTextWithoutTrivia();
                if (text.EqualsAny(Constants.Comments.UnusedPhrase, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var issue in AnalyzeParameter(parameter, text))
                {
                    yield return issue;
                }
            }
        }
    }
}