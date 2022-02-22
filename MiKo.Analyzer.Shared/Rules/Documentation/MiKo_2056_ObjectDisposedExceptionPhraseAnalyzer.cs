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

        internal static string GetEndingPhrase(ISymbol symbol) => HasCloseMethod(symbol)
                                                                      ? Constants.Comments.ObjectDisposedExceptionAlternatingEndingPhrase
                                                                      : Constants.Comments.ObjectDisposedExceptionEndingPhrase;

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment)
        {
            // get rid of the para tags as we are not interested into them
            var comment = exceptionComment.GetTextWithoutTrivia().WithoutParaTags().Trim();

            if (comment.EndsWith(Constants.Comments.ObjectDisposedExceptionEndingPhrase, Comparison))
            {
                yield break;
            }

            // alternative check for Closed methods
            if (HasCloseMethod(symbol) && comment.EndsWith(Constants.Comments.ObjectDisposedExceptionAlternatingEndingPhrase, Comparison))
            {
                yield break;
            }

            yield return ExceptionIssue(exceptionComment, Constants.Comments.ObjectDisposedExceptionEndingPhrase);
        }

        private static bool HasCloseMethod(ISymbol symbol) => symbol
                                                              .FindContainingType()
                                                              .GetMembersIncludingInherited<IMethodSymbol>()
                                                              .Any(_ => _.Name == "Close");
    }
}
