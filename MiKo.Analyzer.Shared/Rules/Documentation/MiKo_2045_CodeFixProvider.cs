﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2045_CodeFixProvider)), Shared]
    public sealed class MiKo_2045_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2045";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => XmlText(GetParameter(syntax));

        private static string GetParameter(SyntaxNode original)
        {
            switch (original)
            {
                case XmlElementSyntax e:
                    return GetParameterName(e);

                case XmlEmptyElementSyntax ee:
                    return GetParameterName(ee);

                default:
                    return Constants.TODO;
            }
        }
    }
}