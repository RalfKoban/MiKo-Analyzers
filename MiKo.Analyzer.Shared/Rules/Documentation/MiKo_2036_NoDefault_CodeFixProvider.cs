﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2036_NoDefault_CodeFixProvider)), Shared]
    public sealed class MiKo_2036_NoDefault_CodeFixProvider : MiKo_2036_CodeFixProvider
    {
        protected override string Title => Resources.MiKo_2036_CodeFixTitle_NoDefault;

        protected override bool IsApplicable(in ImmutableArray<Diagnostic> issues) => issues.Any();

        protected override IEnumerable<XmlNodeSyntax> GetDefaultComment(Document document, TypeSyntax returnType)
        {
            return new XmlNodeSyntax[] { XmlText(Constants.Comments.NoDefaultPhrase) };
        }
    }
}