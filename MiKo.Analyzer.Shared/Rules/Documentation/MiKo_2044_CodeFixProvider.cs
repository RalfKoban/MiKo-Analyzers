﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2044_CodeFixProvider)), Shared]
    public sealed class MiKo_2044_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2044";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax != null)
            {
                var cref = syntax.GetCref();
                var name = cref.GetCrefType().GetName();

                return SyntaxFactory.XmlParamRefElement(name);
            }

            return null;
        }
    }
}