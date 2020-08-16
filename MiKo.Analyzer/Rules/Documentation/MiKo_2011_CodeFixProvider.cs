﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2011_CodeFixProvider)), Shared]
    public sealed class MiKo_2011_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2011_UnsealedClassSummaryAnalyzer.Id;

        protected override string Title => "Remove sealed text";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(Constants.XmlTag.Summary, syntaxNodes).First();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            return SyntaxFactory.XmlElement(comment.StartTag, comment.WithoutText(Constants.Comments.SealedClassPhrase), comment.EndTag);
        }
    }
}