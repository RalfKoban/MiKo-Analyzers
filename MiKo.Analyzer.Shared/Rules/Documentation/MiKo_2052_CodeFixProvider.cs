using System;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2052_CodeFixProvider)), Shared]
    public sealed class MiKo_2052_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2052";

        protected override async Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var symbol = syntax.GetEnclosingSymbol(semanticModel);

            var updatedSyntax = syntax.ReplaceNodes(
                                                syntax.GetExceptionXmls().Where(_ => _.IsExceptionCommentFor<ArgumentNullException>()),
                                                (_, rewritten) => GetFixedExceptionCommentForArgumentNullException(rewritten, symbol));

            return updatedSyntax;
        }
    }
}