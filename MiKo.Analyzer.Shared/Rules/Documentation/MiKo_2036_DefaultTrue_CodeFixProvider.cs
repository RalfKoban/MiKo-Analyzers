﻿using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2036_DefaultTrue_CodeFixProvider)), Shared]
    public sealed class MiKo_2036_DefaultTrue_CodeFixProvider : MiKo_2036_CodeFixProvider
    {
        protected override string Title => Resources.MiKo_2036_CodeFixTitle_DefaultTrue;

        protected override IEnumerable<XmlNodeSyntax> GetDefaultComment(Document document, TypeSyntax returnType)
        {
            return new XmlNodeSyntax[]
                       {
                           XmlText(Constants.Comments.DefaultStartingPhrase),
                           SeeLangword_True(),
                           XmlText("."),
                       };
        }
    }
}