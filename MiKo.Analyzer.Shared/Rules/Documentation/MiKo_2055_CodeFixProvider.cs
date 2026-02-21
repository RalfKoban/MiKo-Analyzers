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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2055_CodeFixProvider)), Shared]
    public sealed class MiKo_2055_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2055";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = syntax.ReplaceNodes(
                                                syntax.GetExceptionXmls().Where(_ => _.IsExceptionCommentFor<ArgumentOutOfRangeException>()),
                                                (_, rewritten) => GetFixedExceptionCommentForArgumentOutOfRangeException(rewritten));

            return Task.FromResult(updatedSyntax);
        }
    }
}