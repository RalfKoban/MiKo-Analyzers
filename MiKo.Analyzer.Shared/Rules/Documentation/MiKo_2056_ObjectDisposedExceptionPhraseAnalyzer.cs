using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2056";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer() : base(Id, typeof(ObjectDisposedException))
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment)
        {
            // get rid of the para tags as we are not interested into them
            var comment = exceptionComment.GetTextTrimmed();

            if (comment.EndsWith(Constants.Comments.ObjectDisposedExceptionEndingPhrase, Comparison))
            {
                yield break;
            }

            var hasCloseMethod = HasCloseMethod(symbol);

            if (hasCloseMethod && comment.EndsWith(Constants.Comments.ObjectDisposedExceptionAlternatingEndingPhrase, Comparison))
            {
                // allowed alternative for Closed methods
                yield break;
            }

            var endingPhrase = hasCloseMethod
                               ? Constants.Comments.ObjectDisposedExceptionAlternatingEndingPhrase
                               : Constants.Comments.ObjectDisposedExceptionEndingPhrase;

            yield return ExceptionIssue(exceptionComment, endingPhrase);
        }

        private static bool HasCloseMethod(ISymbol symbol) => symbol.FindContainingType().GetMembersIncludingInherited<IMethodSymbol>("Close").Any();
    }
}
