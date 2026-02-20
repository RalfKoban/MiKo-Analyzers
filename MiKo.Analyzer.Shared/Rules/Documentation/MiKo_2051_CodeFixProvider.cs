using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2051_CodeFixProvider)), Shared]
    public sealed class MiKo_2051_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2051";

        protected override async Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var symbol = syntax.GetEnclosingSymbol(semanticModel);

            var updatedSyntax = syntax.ReplaceNodes(
                                                syntax.GetExceptionXmls(),
                                                (_, rewritten) =>
                                                                 {
                                                                     if (rewritten.IsExceptionCommentFor<ArgumentNullException>())
                                                                     {
                                                                         return GetFixedExceptionCommentForArgumentNullException(rewritten, symbol);
                                                                     }

                                                                     if (rewritten.IsExceptionCommentFor<ObjectDisposedException>())
                                                                     {
                                                                         return rewritten.WithContent(XmlText(Constants.Comments.ObjectDisposedExceptionPhrase).WithLeadingXmlComment().WithTrailingXmlComment());
                                                                     }

                                                                     if (rewritten.IsExceptionCommentFor<ArgumentOutOfRangeException>())
                                                                     {
                                                                         return GetFixedExceptionCommentForArgumentOutOfRangeException(rewritten);
                                                                     }

                                                                     if (rewritten.IsExceptionCommentFor<ArgumentException>())
                                                                     {
                                                                         return GetFixedExceptionCommentForArgumentException(rewritten);
                                                                     }

                                                                     return GetFixedStartingPhrase(rewritten);
                                                                 });

            return updatedSyntax;
        }
    }
}