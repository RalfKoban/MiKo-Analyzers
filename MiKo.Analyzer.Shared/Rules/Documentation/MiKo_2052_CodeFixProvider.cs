﻿using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2052_CodeFixProvider)), Shared]
    public sealed class MiKo_2052_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2052";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
        {
            var updatedSyntax = syntax.ReplaceNodes(
                                                syntax.GetExceptionXmls().Where(_ => _.IsExceptionCommentFor<ArgumentNullException>()),
                                                (_, rewritten) => GetFixedExceptionCommentForArgumentNullException(document, rewritten));

            return updatedSyntax;
        }
    }
}