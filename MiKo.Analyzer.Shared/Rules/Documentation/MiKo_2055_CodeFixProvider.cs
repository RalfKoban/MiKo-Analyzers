using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2055_CodeFixProvider)), Shared]
    public sealed class MiKo_2055_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2055";

        protected override string Title => Resources.MiKo_2055_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var updatedSyntax = syntax.ReplaceNodes(
                                                syntax.GetExceptionXmls().Where(_ => _.IsExceptionCommentFor<ArgumentOutOfRangeException>()),
                                                (_, rewritten) => GetFixedExceptionCommentForArgumentOutOfRangeException(rewritten));

            return updatedSyntax;
        }
    }
}