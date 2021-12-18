using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2054_CodeFixProvider)), Shared]
    public sealed class MiKo_2054_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2054_ArgumentExceptionPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2054_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var updatedSyntax = syntax.ReplaceNodes(
                                                    syntax.GetExceptionXmls().Where(_ => _.IsExceptionCommentFor<ArgumentException>()),
                                                    (original, rewritten) => GetFixedExceptionCommentForArgumentException(original));

            return updatedSyntax;
        }
    }
}